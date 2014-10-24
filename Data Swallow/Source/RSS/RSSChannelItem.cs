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
using System.Text;
using YAXLib;
namespace DataSwallow.Source.RSS
{
    /// <summary>
    /// Represents an RSS Channel Item
    /// </summary>
    public sealed class RSSChannelItem : IEquatable<RSSChannelItem>
    {
        /// <summary>
        /// Gets or sets the author.
        /// </summary>
        /// <value>
        /// The author.
        /// </value>
        [YAXSerializeAs("author")]
        [YAXErrorIfMissed(YAXExceptionTypes.Ignore)]
        public string Author { get; set; }
        /// <summary>
        /// Gets or sets the category.
        /// </summary>
        /// <value>
        /// The category.
        /// </value>
        [YAXSerializeAs("category")]
        [YAXErrorIfMissed(YAXExceptionTypes.Error)]
        public string Category { get; set; }
        /// <summary>
        /// Gets or sets the comments.
        /// </summary>
        /// <value>
        /// The comments.
        /// </value>
        [YAXSerializeAs("comments")]
        [YAXErrorIfMissed(YAXExceptionTypes.Ignore)]
        public string Comments { get; set; }
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        [YAXSerializeAs("description")]
        [YAXErrorIfMissed(YAXExceptionTypes.Error)]
        public string Description { get; set; }
        /// <summary>
        /// Gets or sets the enclosure.
        /// </summary>
        /// <value>
        /// The enclosure.
        /// </value>
        [YAXSerializeAs("enclosure")]
        [YAXErrorIfMissed(YAXExceptionTypes.Ignore)]
        public string Enclosure { get; set; }
        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        /// <value>
        /// The unique identifier.
        /// </value>
        [YAXSerializeAs("guid")]
        [YAXErrorIfMissed(YAXExceptionTypes.Error)]
        public string Guid { get; set; }
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
        /// Gets or sets the publication date.
        /// </summary>
        /// <value>
        /// The publication date.
        /// </value>
        [YAXSerializeAs("pubDate")]
        [YAXErrorIfMissed(YAXExceptionTypes.Error)]
        public string PublicationDate { get; set; }
        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        /// <value>
        /// The source.
        /// </value>
        [YAXSerializeAs("source")]
        [YAXErrorIfMissed(YAXExceptionTypes.Ignore)]
        public string Source { get; set; }
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        [YAXSerializeAs("title")]
        [YAXErrorIfMissed(YAXExceptionTypes.Error)]
        public string Title { get; set; }

        private bool EqualsPreamble(object other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (GetType() != other.GetType()) return false;

            return true;
        }

        private int CalculateHashCode(object obj)
        {
            if(obj == null)
            {
                return 0;
            }

            return obj.GetHashCode();
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        public bool Equals(RSSChannelItem other)
        {
            if(EqualsPreamble(other) == false)
            {
                return false;
            }

            return Equals(Author, other.Author)
                && Equals(Category, other.Category)
                && Equals(Comments, other.Comments)
                && Equals(Description, other.Description)
                && Equals(Enclosure, other.Enclosure)
                && Equals(Guid, other.Guid)
                && Equals(Link, other.Link)
                && Equals(PublicationDate, other.PublicationDate)
                && Equals(Source, other.Source)
                && Equals(Title, other.Title);
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

            return Equals(obj as RSSChannelItem);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return CalculateHashCode(Author)
                ^ CalculateHashCode(Category)
                ^ CalculateHashCode(Comments)
                ^ CalculateHashCode(Description)
                ^ CalculateHashCode(Enclosure)
                ^ CalculateHashCode(Guid)
                ^ CalculateHashCode(Link)
                ^ CalculateHashCode(PublicationDate)
                ^ CalculateHashCode(Source)
                ^ CalculateHashCode(Title);
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
                .AppendLine("Author: " + Author)
                .AppendLine("Category: " + Category)
                .AppendLine("Comments: " + Comments)
                .AppendLine("Description: " + Description)
                .AppendLine("Enclosure: " + Enclosure)
                .AppendLine("Guid: " + Guid)
                .AppendLine("Link: " + Link)
                .AppendLine("Publication Date: " + PublicationDate)
                .AppendLine("Source: " + Source)
                .AppendLine("Title: " + Title);

            return builder.ToString();
        }
    }
}
