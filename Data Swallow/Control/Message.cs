﻿namespace DataSwallow.Control
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
