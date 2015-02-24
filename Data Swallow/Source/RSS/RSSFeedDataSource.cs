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
using log4net;
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

        private enum MessageType { Start, Stop, Pause, Resume, AddOutputStream, Fetch }

        private sealed class MessagePayload
        {
            public IOutputStream<RSSFeed> OutputStream { get; set; }
        }
        #endregion

        #region private fields
        private const int DefaultVariability = 4;
        private const int DefaultPauseTimeInSeconds = 60;

        private static readonly Message<MessageType, MessagePayload> StartMessage = new Message<MessageType, MessagePayload>(MessageType.Start, new MessagePayload(), "Start");
        private static readonly Message<MessageType, MessagePayload> StopMessage = new Message<MessageType, MessagePayload>(MessageType.Stop, new MessagePayload(), "Stop");
        private static readonly Message<MessageType, MessagePayload> PauseMessage = new Message<MessageType, MessagePayload>(MessageType.Pause, new MessagePayload(), "Pause");
        private static readonly Message<MessageType, MessagePayload> ResumeMessage = new Message<MessageType, MessagePayload>(MessageType.Resume, new MessagePayload(), "Resume");
        private static readonly Message<MessageType, MessagePayload> FetchMessage = new Message<MessageType, MessagePayload>(MessageType.Fetch, new MessagePayload(), "Fetch");

        private static readonly ILog Logger = LogManager.GetLogger(typeof(RSSFeedDataSource));

        private readonly Uri _feedUrl;
        private readonly int _pauseTime;
        private readonly int _variability;
        private readonly Random _waitSeed;
        private readonly HttpClient _client;
        private readonly YAXSerializer _serializer;
        private readonly FunctionalStatelessActor<Message<MessageType, MessagePayload>> _actorEngine;
        private readonly ISet<IOutputStream<RSSFeed>> _outputStreams;
        private readonly CancellationTokenSource _stopTokenSource;

        private State _state;
        private bool _isDisposed;
        #endregion

        #region ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="RSSFeedDataSource"/> class.
        /// </summary>
        /// <param name="feedUrl">The feed URL.</param>
        /// <param name="pauseTime">The pause time in seconds.</param>
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
            _outputStreams = new HashSet<IOutputStream<RSSFeed>>();
            _stopTokenSource = new CancellationTokenSource();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RSSFeedDataSource"/> class.
        /// </summary>
        /// <param name="feedUrl">The feed URL.</param>
        /// <param name="pauseTime">The pause time in seconds.</param>
        public RSSFeedDataSource(Uri feedUrl, int pauseTime)
            : this(feedUrl, pauseTime, DefaultVariability)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RSSFeedDataSource"/> class.
        /// </summary>
        /// <param name="feedUrl">The feed URL.</param>
        public RSSFeedDataSource(Uri feedUrl)
            : this(feedUrl, DefaultPauseTimeInSeconds)
        {
        }
        #endregion

        #region public methods
        /// <summary>
        /// Starts this instance.
        /// </summary>
        /// <returns>
        /// A Task representing the starting of this instance
        /// </returns>
        /// <exception cref="System.ObjectDisposedException">RSSFeedDataSource</exception>
        public void Start()
        {
            AssertNotDisposed();

            Logger.Debug("Starting RSSFeedDataSource");

            _actorEngine.Start();
            _actorEngine.Post(StartMessage);
        }

        /// <summary>
        /// Resumes this instance.
        /// </summary>
        /// <returns>A Task representing the resuming of this instance</returns>
        public void Resume()
        {
            AssertNotDisposed();

            Logger.Debug("Resuming RSSFeedDataSource");

            _actorEngine.Post(ResumeMessage);
        }

        /// <summary>
        /// Pauses this instance.
        /// </summary>
        /// <returns>A Task representing the pausing of this instance</returns>
        /// <exception cref="System.ObjectDisposedException">RSSFeedDataSource</exception>
        public void Pause()
        {
            AssertNotDisposed();

            Logger.Debug("Pausing RSSFeedDataSource");

            _actorEngine.Post(PauseMessage);
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        /// <returns>A Task representing the stopping of this instance</returns>
        /// <exception cref="System.ObjectDisposedException">RSSFeedDataSource</exception>
        public void Stop()
        {
            AssertNotDisposed();

            Logger.Debug("Stopping RSSFeedDataSource");

            _actorEngine.Post(StopMessage);

            _stopTokenSource.Cancel();
        }

        /// <summary>
        /// Blocks the current thread, awaiting for all messages to be processed. Call <see cref="Stop()" /> before calling this.
        /// </summary>
        public void AwaitTermination()
        {
            AssertNotDisposed();

            Logger.Debug("Awaiting Termination of RSSFeedDataSource");

            _actorEngine.AwaitTermination();
        }

        /// <summary>
        /// Adds the output stream asynchronous.
        /// </summary>
        /// <param name="outputStream">The output stream.</param>
        /// <exception cref="System.ObjectDisposedException">RSSFeedDataSource</exception>
        public void AddOutputStream(IOutputStream<RSSFeed> outputStream)
        {
            AssertNotDisposed();

            _actorEngine.Post(CreateAddOutputStream(outputStream));
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
            _stopTokenSource.Dispose();
        }
        #endregion

        #region private methods
        private void AssertNotDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException("RSSFeedDataSource");
            }
        }

        private void HandleAddOutputStreamMessage(Message<MessageType, MessagePayload> message)
        {
            _outputStreams.Add(message.Payload.OutputStream);
        }

        private void HandleFetchMessage()
        {
            if (_state != State.Playing) return;

            try
            {
                var waitInSeconds = _waitSeed.Next(_variability) + _pauseTime;
                var waitInTimeSpan = TimeSpan.FromSeconds(waitInSeconds);

                Logger.DebugFormat("Waiting {0} seconds until next RSS fetch", waitInSeconds);

                Task.Delay(waitInTimeSpan, _stopTokenSource.Token).Wait();

                Logger.DebugFormat("RSSFeedDataSource fetching RSS feed: {0}", _feedUrl);

                var contents = _client.GetStringAsync(_feedUrl).Result;
                var rssFeed = _serializer.Deserialize(contents) as RSSFeed;
                if (rssFeed != null)
                {
                    foreach (var outputStream in _outputStreams)
                    {
                        outputStream.Post(rssFeed);
                    }
                }
            }
            catch (AggregateException e)
            {
                if(e.Flatten().InnerException.GetType().IsAssignableFrom(typeof(TaskCanceledException)))
                {
                    Logger.Debug("RSSFeedDataSource cancelled");
                }
                else
                {
                    Logger.Error("An error occurred in the RSSFeedDataSource", e);
                }
            }
            catch (Exception e)
            {
                Logger.Error("An exception occurred while fetching the RSS Feed!", e);
            }

            //Keep looping and reposting this message
            _actorEngine.Post(FetchMessage);
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
                _actorEngine.Post(FetchMessage);
            }
        }

        private void HandleStartMessage()
        {
            if (_state == State.HasNotStarted)
            {
                _state = State.Playing;
                _actorEngine.Post(FetchMessage);
            }
        }

        private void HandleStopMessage()
        {
            _state = State.Stopped;
            _actorEngine.Stop();
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
                    Logger.ErrorFormat("Received unknown message {0}", message.MessageType);
                    throw new InvalidOperationException("Unknown message type");
            }
        }

        private static Message<MessageType, MessagePayload> CreateAddOutputStream(IOutputStream<RSSFeed> outputStream)
        {
            var payload = new MessagePayload
            {
                OutputStream = outputStream,
            };

            return new Message<MessageType, MessagePayload>(MessageType.AddOutputStream, payload, "Add Output Stream");
        }
        #endregion
    }
}
