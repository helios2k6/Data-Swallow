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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YAXLib;

namespace DataSwallow.Source.RSS
{
    /// <summary>
    /// Represents an RSS Feed Channel
    /// </summary>
    public sealed class RSSChannel : IEquatable<RSSChannel>
    {
        #region ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="RSSChannel"/> class.
        /// </summary>
        public RSSChannel()
        {
            Description = string.Empty;
            Link = string.Empty;
            Title = string.Empty;
            Items = new RSSChannelItem[0];
        }
        #endregion

        #region public properties
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        [YAXSerializeAs("description")]
        [YAXErrorIfMissed(YAXExceptionTypes.Ignore)]
        public string Description { get; set; }
        /// <summary>
        /// Gets or sets the link.
        /// </summary>
        /// <value>
        /// The link.
        /// </value>
        [YAXSerializeAs("link")]
        [YAXErrorIfMissed(YAXExceptionTypes.Error)]
        public string Link { get; set; }
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        [YAXSerializeAs("title")]
        [YAXErrorIfMissed(YAXExceptionTypes.Error)]
        public string Title { get; set; }
        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        [YAXCollection(YAXCollectionSerializationTypes.RecursiveWithNoContainingElement, EachElementName = "item")]
        [YAXErrorIfMissed(YAXExceptionTypes.Error)]
        public RSSChannelItem[] Items { get; set; }
        #endregion

        #region public methods
        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public bool Equals(RSSChannel other)
        {
            if (EqualsPreamble(other) == false)
            {
                return false;
            }

            return Equals(Description, other.Description)
                && Equals(Link, other.Link)
                && Equals(Title, other.Title)
                && Enumerable.SequenceEqual(Items, other.Items);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (EqualsPreamble(obj) == false)
            {
                return false;
            }

            return Equals(obj as RSSChannel);
        }

        private int CalculateHashCode(object obj)
        {
            if (obj == null)
            {
                return 0;
            }

            return obj.GetHashCode();
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return CalculateHashCode(Description)
                ^ CalculateHashCode(Link)
                ^ Items.GetSequenceHashCode();
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var builder = new StringBuilder();

            builder
                .AppendLine("Title: " + Title)
                .AppendLine("Description: " + Description)
                .AppendLine("Link: " + Link);

            if (Items != null)
            {
                builder.AppendLine("Items").AppendLine("{");
                foreach (var i in Items.Where(t => t != null))
                {
                    builder.AppendLine("\t" + i.ToString());
                }
            }
            else
            {
                builder.AppendLine("No Items");
            }

            return builder.ToString();
        }
        #endregion

        #region private methods
        private bool EqualsPreamble(object other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (GetType() != other.GetType()) return false;

            return true;
        }
        #endregion
    }
}
