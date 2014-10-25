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
using DataSwallow.Source.RSS;
using DataSwallow.Stream;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataSwallow.Filter.Anime
{
    /// <summary>
    /// A filter that can detect anime entries using an RSSFeed
    /// </summary>
    public sealed class RSSAnimeDetectionFilter : IFilter<RSSFeed, AnimeEntry>, IDisposable
    {
        #region private classes
        private enum MessageType { Accept, AddOutputStream, GetOutputStreams }

        private sealed class MessagePayload
        {
            public IOutputStream<AnimeEntry> OutputStream { get; set; }
            public int PortNumber { get; set; }

            public TaskCompletionSource<IEnumerable<Tuple<IOutputStream<AnimeEntry>, int>>> TCS { get; set; }

            public IOutputStreamMessage<RSSFeed> Message { get; set; }
        }
        #endregion

        #region private fields
        private readonly FunctionalStatelessActor<Message<MessageType, MessagePayload>> _actorEngine;
        private readonly IDictionary<int, IOutputStream<AnimeEntry>> _outputStreams;

        private bool _isDisposed;
        #endregion

        #region ctor
        public RSSAnimeDetectionFilter()
        {
            _actorEngine = new FunctionalStatelessActor<Message<MessageType, MessagePayload>>(Process);
            _outputStreams = new Dictionary<int, IOutputStream<AnimeEntry>>();
        }
        #endregion

        #region public methods
        public override string ToString()
        {
            return "RSS Anime Detection Filter";
        }

        public override bool Equals(object other)
        {
            return ReferenceEquals(this, other);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public async Task AddOutputStreamAsync(IOutputStream<AnimeEntry> outputStream, int sourcePortNumber)
        {
            if (_isDisposed) throw new ObjectDisposedException("RSSAnimeDetectionFilter");

            await _actorEngine.PostAndReplyAsync(CreateAddOutputStream(outputStream, sourcePortNumber));
        }

        public Task<IEnumerable<Tuple<IOutputStream<AnimeEntry>, int>>> GetOutputStreamsAsync()
        {
            if (_isDisposed) throw new ObjectDisposedException("RSSAnimeDetectionFilter");

            var tcs = new TaskCompletionSource<IEnumerable<Tuple<IOutputStream<AnimeEntry>, int>>>();
            var message = CreateGetOutputStreamsMessage(tcs);

            _actorEngine.Post(message);

            return tcs.Task;
        }

        public async Task AcceptAsync(IOutputStreamMessage<RSSFeed> message)
        {
            if (_isDisposed) throw new ObjectDisposedException("RSSAnimeDetectionFilter");

            await _actorEngine.PostAndReplyAsync(CreateAcceptAsyncMessage(message));
        }

        public void Dispose()
        {
            if (_isDisposed) return;

            _isDisposed = true;

            _actorEngine.Dispose();
        }
        #endregion

        #region private methods
        private void HandleAcceptMessage(Message<MessageType, MessagePayload> message)
        {
            //Luckily, we don't have any input ports, so just ignore it
            RSSFeed feed = message.Payload.Message.Payload;

            IEnumerable<AnimeEntry> entries;
            if(RSSFeedProcessor.Instance.TryGetAnimeEntries(feed, out entries))
            {
                foreach(var entry in entries)
                {
                    foreach (var stream in _outputStreams)
                    {
                        stream.Value.PutAsync(entry);
                    }
                }
            }
        }

        private void HandleAddOutputStreamMessage(Message<MessageType, MessagePayload> message)
        {
            _outputStreams[message.Payload.PortNumber] = message.Payload.OutputStream;
        }

        private void HandleGetOutputStreamsMessage(Message<MessageType, MessagePayload> message)
        {
            var list = new List<Tuple<IOutputStream<AnimeEntry>, int>>();

            foreach (var kvp in _outputStreams)
            {
                list.Add(Tuple.Create(kvp.Value, kvp.Key));
            }

            message.Payload.TCS.TrySetResult(list);
        }

        private void Process(Message<MessageType, MessagePayload> message)
        {
            switch(message.MessageType)
            {
                case MessageType.Accept:
                    HandleAcceptMessage(message);
                    break;
                case MessageType.AddOutputStream:
                    HandleAddOutputStreamMessage(message);
                    break;
                case MessageType.GetOutputStreams:
                    HandleGetOutputStreamsMessage(message);
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        private static Message<MessageType, MessagePayload> CreateAcceptAsyncMessage(IOutputStreamMessage<RSSFeed> message)
        {
            var payload = new MessagePayload
            {
                Message = message
            };

            return new Message<MessageType, MessagePayload>(MessageType.Accept, payload, "Accept");
        }

        private static Message<MessageType, MessagePayload> CreateAddOutputStream(IOutputStream<AnimeEntry> outputStream, int sourcePort)
        {
            var payload = new MessagePayload
            {
                OutputStream = outputStream,
                PortNumber = sourcePort
            };

            return new Message<MessageType, MessagePayload>(MessageType.AddOutputStream, payload, "Add Output Stream");
        }

        private static Message<MessageType, MessagePayload> CreateGetOutputStreamsMessage(TaskCompletionSource<IEnumerable<Tuple<IOutputStream<AnimeEntry>, int>>> tcs)
        {
            var payload = new MessagePayload
            {
                TCS = tcs
            };

            return new Message<MessageType,MessagePayload>(MessageType.GetOutputStreams, payload, "Get Output Streams");
        }
        #endregion
    }
}
