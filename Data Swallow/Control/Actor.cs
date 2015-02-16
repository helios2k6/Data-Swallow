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

using log4net;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DataSwallow.Control
{
    /// <summary>
    /// The base class for all Actors. This class can only be started once!
    /// </summary>
    /// <typeparam name="TMessage">The type of the message.</typeparam>
    public abstract class Actor<TMessage> : IActor<TMessage>, IDisposable
    {
        #region private classes
        private sealed class Ticket
        {
            public TMessage Message { get; set; }
            public TaskCompletionSource<object> TCS { get; set; }
        }
        #endregion

        #region private fields
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Actor<TMessage>));

        private readonly BlockingCollection<Ticket> _messages = new BlockingCollection<Ticket>();
        private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();
        private readonly EventWaitHandle _shutdownSignal = new EventWaitHandle(false, EventResetMode.ManualReset);

        private bool _isDisposed;
        private bool _alreadyStartedOnce;
        #endregion

        #region public properties
        /// <summary>
        /// Gets a value indicating whether this instance is disposed.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is disposed; otherwise, <c>false</c>.
        /// </value>
        public bool IsDisposed { get { return _isDisposed; } }

        /// <summary>
        /// Gets the stop token. This token represents the cancellation token that this actor
        /// is using to signal that it has been stopped.
        /// </summary>
        /// <value>
        /// The stop token.
        /// </value>
        public CancellationToken StopToken { get { return _tokenSource.Token; } }
        #endregion

        #region public methods
        /// <summary>
        /// Posts the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Post(TMessage message)
        {
            AssertNotDisposed();

            var ticket = new Ticket
            {
                Message = message,
                TCS = new TaskCompletionSource<object>(),
            };

            AddToMessagePool(ticket);
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public void Stop()
        {
            AssertNotDisposed();

            _tokenSource.Cancel();
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        /// <returns>A Task representing the running of this instance</returns>
        public void Start()
        {
            AssertNotDisposed();

            if (_alreadyStartedOnce)
            {
                Logger.Error("The logger has already been started. It cannot be started twice.");
                throw new InvalidOperationException("This actor has already been started once.");
            }

            _alreadyStartedOnce = true;
            Task.Factory.StartNew(Run);
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
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Blocks the current thread, awaiting for all messages to be processed. Call <see cref="Stop()"/> before calling this.
        /// </summary>
        /// <exception cref="System.ObjectDisposedException">Actor</exception>
        public void AwaitTermination()
        {
            AssertNotDisposed();

            _shutdownSignal.WaitOne();
        }
        #endregion

        #region protected methods
        /// <summary>
        /// The function to call before processing the message.
        /// </summary>
        /// <param name="message">The message.</param>
        protected abstract void PreProcessMessage(TMessage message);
        /// <summary>
        /// Process the message.
        /// </summary>
        /// <param name="message">The message.</param>
        protected abstract void ProcessMessage(TMessage message);
        /// <summary>
        /// The function to call after processing the message.
        /// </summary>
        /// <param name="message">The message.</param>
        protected abstract void PostProcessMessage(TMessage message);

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="isDisposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool isDisposing)
        {
            if (_isDisposed) return;

            if (isDisposing)
            {
                Stop();
                CancelRemainingMessages();

                if (_messages != null) _messages.Dispose();
                if (_tokenSource != null) _tokenSource.Dispose();
                if (_shutdownSignal != null) _shutdownSignal.Dispose();

                _isDisposed = true;
            }
        }
        #endregion

        #region private methods
        private void AddToMessagePool(Ticket ticket)
        {
            _messages.TryAdd(ticket, 100, _tokenSource.Token);
        }

        private void AssertNotDisposed()
        {
            if (_isDisposed)
            {
                Logger.Error("The actor received a message after being disposed.");
                throw new ObjectDisposedException("Actor");
            }
        }

        private void CancelRemainingMessages()
        {
            foreach (var ticket in _messages)
            {
                ticket.TCS.TrySetCanceled();
            }
        }

        private void Run()
        {
            try
            {
                foreach (var ticket in _messages.GetConsumingEnumerable(_tokenSource.Token))
                {
                    if (EqualityComparer<Ticket>.Default.Equals(ticket, default(Ticket)))
                    {
                        continue;
                    }

                    try
                    {
                        var messageBody = ticket.Message;

                        PreProcessMessage(messageBody);
                        ProcessMessage(messageBody);
                        PostProcessMessage(messageBody);

                        ticket.TCS.TrySetResult(null);
                    }
                    catch (Exception e)
                    {
                        Logger.Error("An exception occurred during an Actor task", e);
                        ticket.TCS.TrySetException(e);
                    }
                }
            }
            catch (OperationCanceledException canceledException)
            {
                Logger.Info("Actor task loop was cancelled", canceledException);
            }
            
            Logger.Debug("Actor shutting down");
            CancelRemainingMessages();
            _messages.CompleteAdding();
            _shutdownSignal.Set();
        }

        private void CancelRemainingTasks()
        {
            Ticket ticket;
            while (_messages.TryTake(out ticket))
            {
                if (ticket.TCS != null)
                {
                    ticket.TCS.TrySetCanceled();
                }
            }
        }
        #endregion
    }
}
