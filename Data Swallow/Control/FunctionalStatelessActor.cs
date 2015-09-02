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
    /// A stateless actor that uses functions to drive its processing
    /// </summary>
    /// <typeparam name="TMessage">The type of the message.</typeparam>
    public sealed class FunctionalStatelessActor<TMessage> : Actor<TMessage>
    {
        #region private fields
        private readonly Action<TMessage> _preAction;
        private readonly Action<TMessage> _processAction;
        private readonly Action<TMessage> _postAction;
        #endregion

        #region ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionalStatelessActor{TMessage}"/> class.
        /// </summary>
        /// <param name="preAction">The pre action.</param>
        /// <param name="processAction">The process action.</param>
        /// <param name="postAction">The post action.</param>
        public FunctionalStatelessActor(Action<TMessage> preAction, Action<TMessage> processAction, Action<TMessage> postAction)
        {
            _preAction = preAction;
            _processAction = processAction;
            _postAction = postAction;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionalStatelessActor{TMessage}"/> class.
        /// </summary>
        /// <param name="processAction">The process action.</param>
        public FunctionalStatelessActor(Action<TMessage> processAction)
            : this(Functions.GetNoOpAction<TMessage>(), processAction, Functions.GetNoOpAction<TMessage>())
        {
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
            return "Functional Stateless Actor";
        }
        #endregion

        #region protected methods
        /// <summary>
        /// The function to call before processing the message.
        /// </summary>
        /// <param name="message">The message.</param>
        protected override void PreProcessMessage(TMessage message)
        {
            _preAction.Invoke(message);
        }

        /// <summary>
        /// Processes the message.
        /// </summary>
        /// <param name="message">The message.</param>
        protected override void ProcessMessage(TMessage message)
        {
            _processAction.Invoke(message);
        }

        /// <summary>
        /// The function to call after processing the message
        /// </summary>
        /// <param name="message">The message.</param>
        protected override void PostProcessMessage(TMessage message)
        {
            _postAction.Invoke(message);
        }
        #endregion
    }
}
