﻿/*
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

using DataSwallow.Anime;
using DataSwallow.Control;
using DataSwallow.Source.RSS;
using DataSwallow.Stream;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataSwallow.Filter.Anime
{
    /// <summary>
    /// A filter that can detect anime entries using an RSSFeed
    /// </summary>
    public sealed class RSSAnimeDetectionFilter : FilterActor<RSSFeed, AnimeEntry>
    {
        #region public methods
        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return "RSS Anime Detection Filter";
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
        /// Digests the message.
        /// </summary>
        /// <param name="feed">The feed.</param>
        /// <param name="portNumber">The port number.</param>
        /// <param name="outputStreams">The output streams.</param>
        protected override void DigestMessage(RSSFeed feed, int portNumber, IEnumerable<KeyValuePair<int, IOutputStream<AnimeEntry>>> outputStreams)
        {
            IEnumerable<AnimeEntry> entries;
            if(RSSFeedProcessor.Instance.TryGetAnimeEntries(feed, out entries))
            {
                foreach(var entry in entries)
                {
                    foreach (var stream in outputStreams)
                    {
                        stream.Value.PutAsync(entry);
                    }
                }
            }
        }
        #endregion
    }
}
