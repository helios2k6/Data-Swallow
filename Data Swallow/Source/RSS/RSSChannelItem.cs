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

using YAXLib;
namespace DataSwallow.Source.RSS
{
    /// <summary>
    /// Represents an RSS Channel Item
    /// </summary>
    public sealed class RSSChannelItem
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
    }
}
