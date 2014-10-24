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

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DataSwallow.Control
{
    /// <summary>
    /// The base class for all Actors
    /// </summary>
    /// <typeparam name="TMessage">The type of the message.</typeparam>
    public abstract class Actor<TMessage> : IActor<TMessage>, IDisposable
    {
        #region private fields
        private readonly BlockingCollection<TMessage> _messages = new BlockingCollection<TMessage>();
        private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();

        private bool _isDisposed;
        #endregion

        #region public methods
        /// <summary>
        /// Posts the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Post(TMessage message)
        {
            _messages.Add(message);
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public void Stop()
        {
            _tokenSource.Cancel();
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        /// <returns>A Task representing the running of this instance</returns>
        public async Task Start()
        {
            while (_tokenSource.IsCancellationRequested == false)
            {
                TMessage nextMessage = await GetNextMessageAsync();

                if (EqualityComparer<TMessage>.Default.Equals(nextMessage, default(TMessage)) == false)
                {
                    PreProcessMessage(nextMessage);
                    ProcessMessage(nextMessage);
                    PostProcessMessage(nextMessage);
                }
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return "Actor Base";
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object other)
        {
            return ReferenceEquals(this, other);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
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
        protected abstract void PreProcessMessage(TMessage message);
        protected abstract void ProcessMessage(TMessage message);
        protected abstract void PostProcessMessage(TMessage message);

        protected virtual void Dispose(bool isDisposing)
        {
            if (_isDisposed) return;

            if(isDisposing)
            {
                if (_messages != null) _messages.Dispose();
                if (_tokenSource != null) _tokenSource.Dispose();

                _isDisposed = true;
            }
        }
        #endregion

        #region private methods
        private Task<TMessage> GetNextMessageAsync()
        {
            return Task.Factory.StartNew<TMessage>(() =>
            {
                try
                {
                    return _messages.Take(_tokenSource.Token);
                }
                catch (TaskCanceledException)
                {
                    return default(TMessage);
                }
            });
        }
        #endregion
    }
}
