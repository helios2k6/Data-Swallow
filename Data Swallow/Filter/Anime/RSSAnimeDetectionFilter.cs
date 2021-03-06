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
using DataSwallow.Source.RSS;
using DataSwallow.Stream;
using System.Collections.Generic;

namespace DataSwallow.Filter.Anime
{
    /// <summary>
    /// A filter that can detect anime entries using an RSSFeed
    /// </summary>
    public sealed class RSSAnimeDetectionFilter : FilterActor<RSSFeed, AnimeEntry>
    {
        #region public static fields
        /// <summary>
        /// The singleton instance of this class
        /// </summary>
        public static readonly RSSAnimeDetectionFilter Instance = new RSSAnimeDetectionFilter();
        #endregion

        #region ctor
        private RSSAnimeDetectionFilter() { }
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
            return "RSS Anime Detection Filter";
        }
        #endregion

        #region protected methods
        /// <summary>
        /// Digests the message.
        /// </summary>
        /// <param name="feed">The feed.</param>
        /// <param name="outputStreams">The output streams.</param>
        protected override void DigestMessage(RSSFeed feed, IEnumerable<IOutputStream<AnimeEntry>> outputStreams)
        {
            IEnumerable<AnimeEntry> entries;
            if (RSSFeedProcessor.TryGetAnimeEntries(feed, out entries))
            {
                foreach (var entry in entries)
                {
                    foreach (var stream in outputStreams)
                    {
                        stream.Post(entry);
                    }
                }
            }
        }
        #endregion
    }
}
