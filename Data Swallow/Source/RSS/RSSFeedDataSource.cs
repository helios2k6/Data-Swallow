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

        private enum MessageType { Start, Stop, Pause, Resume, AddOutputStream, GetOutputStreams, Fetch }

        private sealed class MessagePayload
        {
            public IOutputStream<RSSFeed> OutputStream { get; set; }
            public int PortNumber { get; set; }

            public TaskCompletionSource<IEnumerable<Tuple<IOutputStream<RSSFeed>, int>>> TCS { get; set; }
        }
        #endregion

        #region private fields
        private static readonly Message<MessageType, MessagePayload> StartMessage = new Message<MessageType, MessagePayload>(MessageType.Start, new MessagePayload(), "Start");
        private static readonly Message<MessageType, MessagePayload> StopMessage = new Message<MessageType, MessagePayload>(MessageType.Stop, new MessagePayload(), "Stop");
        private static readonly Message<MessageType, MessagePayload> PauseMessage = new Message<MessageType, MessagePayload>(MessageType.Pause, new MessagePayload(), "Pause");
        private static readonly Message<MessageType, MessagePayload> ResumeMessage = new Message<MessageType, MessagePayload>(MessageType.Resume, new MessagePayload(), "Resume");
        private static readonly Message<MessageType, MessagePayload> FetchMessage = new Message<MessageType, MessagePayload>(MessageType.Fetch, new MessagePayload(), "Fetch");

        private readonly Uri _feedUrl;
        private readonly int _pauseTime;
        private readonly int _variability;
        private readonly Random _waitSeed;
        private readonly HttpClient _client;
        private readonly YAXSerializer _serializer;
        private readonly FunctionalStatelessActor<Message<MessageType, MessagePayload>> _actorEngine;
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

            _actorEngine = new FunctionalStatelessActor<Message<MessageType, MessagePayload>>(Process);
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
        /// <summary>
        /// Gets the output streams asynchronously.
        /// </summary>
        /// <returns>A Task representing the retrieval of the output streams</returns>
        /// <exception cref="System.ObjectDisposedException">RSSFeedDataSource</exception>
        public Task<IEnumerable<Tuple<IOutputStream<RSSFeed>, int>>> GetOutputStreamsAsync()
        {
            if (_isDisposed) throw new ObjectDisposedException("RSSFeedDataSource");

            var tcs = new TaskCompletionSource<IEnumerable<Tuple<IOutputStream<RSSFeed>, int>>>();
            var message = CreateGetOutputStreamsMessage(tcs);

            _actorEngine.PostAsync(message);

            return tcs.Task;
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        /// <returns>
        /// A Task representing the starting of this instance
        /// </returns>
        /// <exception cref="System.ObjectDisposedException">RSSFeedDataSource</exception>
        public void Start()
        {
            if (_isDisposed) throw new ObjectDisposedException("RSSFeedDataSource");

            _actorEngine.Start();
            _actorEngine.PostAsync(StartMessage);
        }

        /// <summary>
        /// Resumes this instance.
        /// </summary>
        /// <returns>A Task representing the resuming of this instance</returns>
        public void Resume()
        {
            if (_isDisposed) throw new ObjectDisposedException("RSSFeedDataSource");

            _actorEngine.PostAndReplyAsync(ResumeMessage);
        }

        /// <summary>
        /// Pauses this instance.
        /// </summary>
        /// <returns>A Task representing the pausing of this instance</returns>
        /// <exception cref="System.ObjectDisposedException">RSSFeedDataSource</exception>
        public void Pause()
        {
            if (_isDisposed) throw new ObjectDisposedException("RSSFeedDataSource");

            _actorEngine.PostAndReplyAsync(PauseMessage);
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        /// <returns>A Task representing the stopping of this instance</returns>
        /// <exception cref="System.ObjectDisposedException">RSSFeedDataSource</exception>
        public void Stop()
        {
            if (_isDisposed) throw new ObjectDisposedException("RSSFeedDataSource");

            _actorEngine.PostAndReplyAsync(StopMessage);
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
        public Task AddOutputStreamAsync(IOutputStream<RSSFeed> outputStream, int sourcePortNumber)
        {
            if (_isDisposed) throw new ObjectDisposedException("RSSFeedDataSource");

            return _actorEngine.PostAndReplyAsync(CreateAddOutputStream(outputStream, sourcePortNumber));
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
        private void HandleAddOutputStreamMessage(Message<MessageType, MessagePayload> message)
        {
            _outputStreams[message.Payload.PortNumber] = message.Payload.OutputStream;
        }

        private void HandleFetchMessage()
        {
            if (_state != State.Playing) return;

            var waitInSeconds = _waitSeed.Next(_variability) + _pauseTime;
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
            _actorEngine.PostAsync(FetchMessage);
        }

        private void HandleGetOutputStreamsMessage(Message<MessageType, MessagePayload> message)
        {
            var streams = new List<Tuple<IOutputStream<RSSFeed>, int>>();

            foreach (var kvp in _outputStreams)
            {
                streams.Add(Tuple.Create(kvp.Value, kvp.Key));
            }

            message.Payload.TCS.TrySetResult(streams);
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
                _actorEngine.PostAsync(FetchMessage);
            }
        }

        private void HandleStartMessage()
        {
            if (_state == State.HasNotStarted)
            {
                _state = State.Playing;
                _actorEngine.PostAsync(FetchMessage);
            }
        }

        private void HandleStopMessage()
        {
            _state = State.Stopped;
        }

        private void Process(Message<MessageType, MessagePayload> message)
        {
            switch (message.MessageType)
            {
                case MessageType.AddOutputStream:
                    HandleAddOutputStreamMessage(message);
                    break;
                case MessageType.Fetch:
                    HandleFetchMessage();
                    break;
                case MessageType.GetOutputStreams:
                    HandleGetOutputStreamsMessage(message);
                    break;
                case MessageType.Pause:
                    HandlePauseMessage();
                    break;
                case MessageType.Resume:
                    HandleResumeMessage();
                    break;
                case MessageType.Start:
                    HandleStartMessage();
                    break;
                case MessageType.Stop:
                    HandleStopMessage();
                    break;
                default:
                    throw new InvalidOperationException("Unknown message type");
            }
        }

        private static Message<MessageType, MessagePayload> CreateAddOutputStream(IOutputStream<RSSFeed> outputStream, int sourcePort)
        {
            var payload = new MessagePayload
            {
                OutputStream = outputStream,
                PortNumber = sourcePort
            };

            return new Message<MessageType, MessagePayload>(MessageType.AddOutputStream, payload, "Add Output Stream");
        }

        private static Message<MessageType, MessagePayload> CreateGetOutputStreamsMessage(TaskCompletionSource<IEnumerable<Tuple<IOutputStream<RSSFeed>, int>>> tcs)
        {
            var payload = new MessagePayload
            {
                TCS = tcs
            };

            return new Message<MessageType, MessagePayload>(MessageType.GetOutputStreams, payload, "Get Output Streams");
        }
        #endregion
    }
}
