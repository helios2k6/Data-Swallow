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

namespace DataSwallow.Control
{
    /// <summary>
    /// Represents a message with a Message Type and Payload
    /// </summary>
    /// <typeparam name="TMessageType">The type of the message type.</typeparam>
    /// <typeparam name="TPayload">The type of the payload.</typeparam>
    public class Message<TMessageType, TPayload>
    {
        #region private fields
        private readonly TMessageType _messageType;
        private readonly TPayload _payload;
        private readonly string _name;
        #endregion

        #region ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="Message{TMessageType, TPayload}"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="payload">The payload.</param>
        /// <param name="name">The name.</param>
        public Message(TMessageType type, TPayload payload, string name)
        {
            _messageType = type;
            _payload = payload;
            _name = name;
        }
        #endregion

        #region public properties
        /// <summary>
        /// Gets the type of the message.
        /// </summary>
        /// <value>
        /// The type of the message.
        /// </value>
        public TMessageType MessageType { get { return _messageType; } }

        /// <summary>
        /// Gets the payload.
        /// </summary>
        /// <value>
        /// The payload.
        /// </value>
        public TPayload Payload { get { return _payload; } }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get { return _name; } }
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
            return string.Format("Message named {0} of type {1} with payload {2}", _name, _messageType, _payload);
        }
        #endregion
    }

    /// <summary>
    /// A convenience class that allows you to send Messages without a payload
    /// </summary>
    /// <typeparam name="TMessageType">The type of the message type.</typeparam>
    public class Message<TMessageType> : Message<TMessageType, object>
    {
        #region ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="Message{TMessageType}"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="name">The name.</param>
        public Message(TMessageType type, string name) : base(type, null, name)
        {
        }
        #endregion
    }
}
