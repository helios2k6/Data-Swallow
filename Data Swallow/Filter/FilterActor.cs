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
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataSwallow.Filter
{
    /// <summary>
    /// The base class for all filter actors
    /// </summary>
    /// <typeparam name="TInput">The type of the input.</typeparam>
    /// <typeparam name="TOutput">The type of the output.</typeparam>
    public abstract class FilterActor<TInput, TOutput> : IFilter<TInput, TOutput>, IDisposable
    {
        #region private classes
        private enum Type { Accept, AddOutputStream }

        private sealed class Payload
        {
            public IOutputStream<TOutput> OutputStream { get; set; }

            public TaskCompletionSource<IEnumerable<Tuple<IOutputStream<TOutput>, int>>> TCS { get; set; }

            public IOutputStreamMessage<TInput> Message { get; set; }
        }
        #endregion

        #region private fields
        private readonly FunctionalStatelessActor<Message<Type, Payload>> _engine;
        private readonly ISet<IOutputStream<TOutput>> _outputStreams;

        private bool _isDisposed;
        #endregion

        #region ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="FilterActor{TInput, TOutput}"/> class.
        /// </summary>
        protected FilterActor()
        {
            _engine = new FunctionalStatelessActor<Message<Type, Payload>>(Process);
            _outputStreams = new HashSet<IOutputStream<TOutput>>();
        }
        #endregion

        #region public methods
        /// <summary>
        /// Starts this instance.
        /// </summary>
        /// <returns></returns>
        public void Start()
        {
            AssertNotDisposed();

            _engine.Start();
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            AssertNotDisposed();

            _engine.Stop();
        }

        /// <summary>
        /// Blocks the current thread, awaiting for all messages to be processed. Call <see cref="Stop()" /> before calling this.
        /// </summary>
        public void AwaitTermination()
        {
            AssertNotDisposed();

            _engine.AwaitTermination();
        }

        /// <summary>
        /// Adds the output stream to the filter
        /// </summary>
        /// <param name="outputStream">The output stream.</param>
        /// <exception cref="System.ObjectDisposedException">RSSAnimeDetectionFilter</exception>
        public void AddOutputStream(IOutputStream<TOutput> outputStream)
        {
            AssertNotDisposed();

            _engine.Post(CreateAddOutputStream(outputStream));
        }

        /// <summary>
        /// Accepts the asynchronous.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>A Task representing this operation</returns>
        /// <exception cref="System.ObjectDisposedException">RSSAnimeDetectionFilter</exception>
        public void Accept(IOutputStreamMessage<TInput> message)
        {
            AssertNotDisposed();

            _engine.Post(CreateAcceptAsyncMessage(message));
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region protected methods
        /// <summary>
        /// Digests the message.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="outputStreams">The output streams.</param>
        protected abstract void DigestMessage(TInput input, IEnumerable<IOutputStream<TOutput>> outputStreams);

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="isDisposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool isDisposing)
        {
            if (_isDisposed) return;

            if (isDisposing)
            {
                _engine.Dispose();
                _isDisposed = true;
            }
        }
        #endregion

        #region private methods
        private void AssertNotDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException("FilterActor");
            }
        }

        private void Process(Message<Type, Payload> message)
        {
            switch (message.MessageType)
            {
                case Type.Accept:
                    HandleAcceptMessage(message.Payload);
                    break;
                case Type.AddOutputStream:
                    HandleAddOutputStreamMessage(message);
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        private void HandleAcceptMessage(Payload payload)
        {
            TInput input = payload.Message.Payload;

            DigestMessage(input, _outputStreams);
        }

        private void HandleAddOutputStreamMessage(Message<Type, Payload> message)
        {
            _outputStreams.Add(message.Payload.OutputStream);
        }

        private static Message<Type, Payload> CreateAcceptAsyncMessage(IOutputStreamMessage<TInput> message)
        {
            var payload = new Payload
            {
                Message = message
            };

            return new Message<Type, Payload>(Type.Accept, payload, "Accept");
        }

        private static Message<Type, Payload> CreateAddOutputStream(IOutputStream<TOutput> outputStream)
        {
            var payload = new Payload
            {
                OutputStream = outputStream,
            };

            return new Message<Type, Payload>(Type.AddOutputStream, payload, "Add Output Stream");
        }
        #endregion
    }
}
