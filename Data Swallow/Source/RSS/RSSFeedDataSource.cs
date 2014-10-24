/*
 * The MIT License (MIT)
 * 
 * Copyright (c) 2014 Andrew B. Johnson
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using DataSwallow.Control;
using DataSwallow.Stream;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using YAXLib;

namespace DataSwallow.Source.RSS
{
    /// <summary>
    /// Represents a Data Source based off a RSS Feed
    /// </summary>
    public sealed class RSSFeedDataSource : ISource<RSSFeed>, IDisposable
    {
        #region private classes
        private enum State { HasNotStarted, Playing, Paused, Stopped }

        private sealed class Message
        {
            public enum MessageType { Start, Stop, Pause, Resume, AddOutputStream, GetOutputStreams, Fetch }

            #region private fields
            private readonly string _name;
            private readonly MessageType _type;
            #endregion

            #region ctor
            private Message(string name, MessageType type)
            {
                _name = name;
                _type = type;
            }
            #endregion

            #region public fields
            public static readonly Message Start = new Message("Start", MessageType.Start);
            public static readonly Message Stop = new Message("Stop", MessageType.Stop);
            public static readonly Message Pause = new Message("Pause", MessageType.Pause);
            public static readonly Message Resume = new Message("Resume", MessageType.Resume);
            public static readonly Message Fetch = new Message("Fetch", MessageType.Fetch);
            #endregion

            #region public properties
            public MessageType Type { get { return _type; } }

            public IOutputStream<RSSFeed> OutputStream { get; private set; }
            public int PortNumber { get; private set; }

            public TaskCompletionSource<IEnumerable<Tuple<IOutputStream<RSSFeed>, int>>> TCS { get; private set; }
            #endregion

            #region public methods
            public static Message CreateGetOutputStreamsMessage(TaskCompletionSource<IEnumerable<Tuple<IOutputStream<RSSFeed>, int>>> tcs)
            {
                var message = new Message("Get Output Streams", MessageType.GetOutputStreams);
                message.TCS = tcs;

                return message;
            }

            public static Message CreateAddOutputStreamMessage(IOutputStream<RSSFeed> stream, int portNumber)
            {
                var message = new Message("Add Output Stream", MessageType.AddOutputStream);
                message.OutputStream = stream;
                message.PortNumber = portNumber;

                return message;
            }

            public override string ToString()
            {
                return _name;
            }

            public override bool Equals(object other)
            {
                return ReferenceEquals(this, other);
            }

            public override int GetHashCode()
            {
                return 0;
            }
            #endregion
        }
        #endregion

        #region private fields
        private readonly Uri _feedUrl;
        private readonly int _pauseTime;
        private readonly int _variability;
        private readonly Random _waitSeed;
        private readonly HttpClient _client;
        private readonly YAXSerializer _serializer;
        private readonly FunctionalStatelessActor<Message> _actorEngine;
        private readonly IDictionary<int, IOutputStream<RSSFeed>> _outputStreams;

        private State _state;
        private bool _isDisposed;
        #endregion

        #region ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="RSSFeedDataSource"/> class.
        /// </summary>
        /// <param name="feedUrl">The feed URL.</param>
        /// <param name="pauseTime">The pause time.</param>
        /// <param name="variability">The variability.</param>
        public RSSFeedDataSource(Uri feedUrl, int pauseTime, int variability)
        {
            _feedUrl = feedUrl;
            _pauseTime = pauseTime;
            _variability = variability;
            _waitSeed = new Random(SystemClock.Instance.Now.InUtc().Second);
            _client = new HttpClient();
            _serializer = new YAXSerializer(typeof(RSSFeed));
            _state = State.HasNotStarted;

            _actorEngine = new FunctionalStatelessActor<Message>(PreProcess, Process, PostProcess);
            _outputStreams = new Dictionary<int, IOutputStream<RSSFeed>>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RSSFeedDataSource"/> class.
        /// </summary>
        /// <param name="feedUrl">The feed URL.</param>
        /// <param name="pauseTime">The pause time.</param>
        public RSSFeedDataSource(Uri feedUrl, int pauseTime)
            : this(feedUrl, pauseTime, 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RSSFeedDataSource"/> class.
        /// </summary>
        /// <param name="feedUrl">The feed URL.</param>
        public RSSFeedDataSource(Uri feedUrl)
            : this(feedUrl, 60, 0)
        {
        }
        #endregion

        #region public methods
        public Task<IEnumerable<Tuple<IOutputStream<RSSFeed>, int>>> GetOutputStreamsAsync()
        {
            if (_isDisposed) throw new ObjectDisposedException("RSSFeedDataSource");

            var tcs = new TaskCompletionSource<IEnumerable<Tuple<IOutputStream<RSSFeed>, int>>>();
            var message = Message.CreateGetOutputStreamsMessage(tcs);

            _actorEngine.Post(message);

            return tcs.Task;
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        /// <returns>
        /// A Task representing the starting of this instance
        /// </returns>
        /// <exception cref="System.ObjectDisposedException">RSSFeedDataSource</exception>
        public async Task Start()
        {
            if (_isDisposed) throw new ObjectDisposedException("RSSFeedDataSource");

            await _actorEngine.Start();
            await _actorEngine.PostAndReplyAsync(Message.Start);
        }

        /// <summary>
        /// Resumes this instance.
        /// </summary>
        /// <returns>A Task representing the resuming of this instance</returns>
        public async Task Resume()
        {
            if (_isDisposed) throw new ObjectDisposedException("RSSFeedDataSource");

            await _actorEngine.Start();
            await _actorEngine.PostAndReplyAsync(Message.Resume);
        }

        /// <summary>
        /// Pauses this instance.
        /// </summary>
        /// <returns>A Task representing the pausing of this instance</returns>
        /// <exception cref="System.ObjectDisposedException">RSSFeedDataSource</exception>
        public async Task Pause()
        {
            if (_isDisposed) throw new ObjectDisposedException("RSSFeedDataSource");

            await _actorEngine.PostAndReplyAsync(Message.Pause);
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        /// <returns>A Task representing the stopping of this instance</returns>
        /// <exception cref="System.ObjectDisposedException">RSSFeedDataSource</exception>
        public async Task Stop()
        {
            if (_isDisposed) throw new ObjectDisposedException("RSSFeedDataSource");

            await _actorEngine.PostAndReplyAsync(Message.Stop);
        }

        /// <summary>
        /// Adds the output stream asynchronous.
        /// </summary>
        /// <param name="outputStream">The output stream.</param>
        /// <param name="sourcePortNumber">The port number.</param>
        /// <returns>
        /// A Task representing the adding of the Output Stream
        /// </returns>
        /// <exception cref="System.ObjectDisposedException">RSSFeedDataSource</exception>
        public async Task AddOutputStreamAsync(IOutputStream<RSSFeed> outputStream, int sourcePortNumber)
        {
            if (_isDisposed) throw new ObjectDisposedException("RSSFeedDataSource");

            await _actorEngine.PostAndReplyAsync(Message.CreateAddOutputStreamMessage(outputStream, sourcePortNumber));
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (_isDisposed) return;

            _isDisposed = true;
            _actorEngine.Dispose();
            _client.Dispose();
        }
        #endregion

        #region private methods
        private void HandleAddOutputStreamMessage(Message message)
        {
            _outputStreams[message.PortNumber] = message.OutputStream;
        }

        private void HandleFetchMessage()
        {
            if (_state != State.Playing) return;

            var waitInSeconds = _waitSeed.Next(_variability);
            var waitInTimeSpan = TimeSpan.FromSeconds(waitInSeconds);

            Thread.Sleep(waitInTimeSpan);

            var getTask = _client.GetStringAsync(_feedUrl);
            var contents = getTask.Result;

            try
            {
                var rssFeed = _serializer.Deserialize(contents) as RSSFeed;
                if(rssFeed != null)
                {
                    foreach (var kvp in _outputStreams)
                    {
                        var stream = kvp.Value;
                        stream.PutAsync(rssFeed);
                    }
                }

                //TODO: if the rss feed is null, then something went wrong (the link could be down or w/e)
                //In that case, we will just reping the server later, so schedule that
            }
            catch(Exception)
            {
                //TODO: Unable to deserialize the stream. Could be from a bad URI. Warn and resechedule
            }

            //Keep looping and reposting this message if everything is good to go
            _actorEngine.Post(Message.Fetch);
        }

        private void HandleGetOutputStreamsMessage(Message message)
        {
            var streams = new List<Tuple<IOutputStream<RSSFeed>, int>>();

            foreach (var kvp in _outputStreams)
            {
                streams.Add(Tuple.Create(kvp.Value, kvp.Key));
            }

            message.TCS.TrySetResult(streams);
        }

        private void HandlePauseMessage()
        {
            _state = State.Paused;
        }

        private void HandleResumeMessage()
        {
            if (_state == State.Paused)
            {
                _state = State.Playing;
                _actorEngine.Post(Message.Fetch);
            }
        }

        private void HandleStartMessage()
        {
            if (_state == State.HasNotStarted)
            {
                _state = State.Playing;
                _actorEngine.Post(Message.Fetch);
            }
        }

        private void HandleStopMessage()
        {
            _state = State.Stopped;
        }

        private void PreProcess(Message message)
        {
        }

        private void Process(Message message)
        {
            switch (message.Type)
            {
                case Message.MessageType.AddOutputStream:
                    HandleAddOutputStreamMessage(message);
                    break;
                case Message.MessageType.Fetch:
                    HandleFetchMessage();
                    break;
                case Message.MessageType.GetOutputStreams:
                    HandleGetOutputStreamsMessage(message);
                    break;
                case Message.MessageType.Pause:
                    HandlePauseMessage();
                    break;
                case Message.MessageType.Resume:
                    HandleResumeMessage();
                    break;
                case Message.MessageType.Start:
                    HandleStartMessage();
                    break;
                case Message.MessageType.Stop:
                    HandleStopMessage();
                    break;
                default:
                    throw new InvalidOperationException("Unknown message type");
            }
        }

        private void PostProcess(Message message)
        {
        }
        #endregion
    }
}
