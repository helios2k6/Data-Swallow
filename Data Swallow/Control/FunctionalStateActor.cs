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

using DataSwallow.Utilities;
using System;

namespace DataSwallow.Control
{
    /// <summary>
    /// A stateful actor that processes messages using the supplied functions and state
    /// </summary>
    /// <typeparam name="TMessage">The type of the message.</typeparam>
    /// <typeparam name="TState">The type of the state.</typeparam>
    public sealed class FunctionalStateActor<TMessage, TState> : Actor<TMessage>
    {
        #region private fields
        private readonly Func<TMessage, TState, TState> _preAction;
        private readonly Func<TMessage, TState, TState> _processAction;
        private readonly Func<TMessage, TState, TState> _postAction;

        private TState _state;
        #endregion

        #region ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionalStateActor{TMessage, TState}"/> class.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <param name="preAction">The pre action.</param>
        /// <param name="processAction">The process action.</param>
        /// <param name="postAction">The post action.</param>
        public FunctionalStateActor(TState state,
            Func<TMessage, TState, TState> preAction,
            Func<TMessage, TState, TState> processAction,
            Func<TMessage, TState, TState> postAction)
        {
            _state = state;
            _preAction = preAction;
            _processAction = processAction;
            _postAction = postAction;
        }
        #endregion

        #region public methods
        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return "Functional State Actor";
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
        #endregion

        #region protected methods
        /// <summary>
        /// The function to call before the message has been processed
        /// </summary>
        /// <param name="message">The message.</param>
        protected override void PreProcessMessage(TMessage message)
        {
            _state = _preAction.Invoke(message, _state);
        }

        /// <summary>
        /// Processes the message.
        /// </summary>
        /// <param name="message">The message.</param>
        protected override void ProcessMessage(TMessage message)
        {
            _state = _processAction.Invoke(message, _state);
        }

        /// <summary>
        /// The function to call after the message has been processed
        /// </summary>
        /// <param name="message">The message.</param>
        protected override void PostProcessMessage(TMessage message)
        {
            _state = _postAction.Invoke(message, _state);
        }
        #endregion
    }
}
