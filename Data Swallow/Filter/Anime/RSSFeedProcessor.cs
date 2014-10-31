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

using DataSwallow.Anime;
using DataSwallow.Source.RSS;
using FansubFileNameParser;
using NodaTime;
using NodaTime.Text;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataSwallow.Filter.Anime
{
    /// <summary>
    /// A RSSFeed -> AnimeEntry processing engine
    /// </summary>
    public sealed class RSSFeedProcessor
    {
        #region private fields
        private static string DateTimePatternRFC822 = "ddd, dd MMM yyyy HH:mm:ss o<+HHmm>";
        #endregion

        #region ctor
        private RSSFeedProcessor()
        {
        }
        #endregion

        #region public properties
        /// <summary>
        /// The singleton instance
        /// </summary>
        public static readonly RSSFeedProcessor Instance = new RSSFeedProcessor();
        #endregion

        #region public methods
        /// <summary>
        /// Tries the get anime entries.
        /// </summary>
        /// <param name="feed">The feed.</param>
        /// <param name="entries">The entries.</param>
        /// <returns>True on success. False otherwise</returns>
        public bool TryGetAnimeEntries(RSSFeed feed, out IEnumerable<AnimeEntry> entries)
        {
            return TryProcessRSSChannel(feed.Channel, out entries);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return "RSS Feed Processor";
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

        #region private methods
        private bool TryProcessRSSChannel(RSSChannel channel, out IEnumerable<AnimeEntry> entries)
        {
            entries = default(IEnumerable<AnimeEntry>);

            var source = channel.Title;
            var results = new List<AnimeEntry>();

            foreach(var item in channel.Items)
            {
                AnimeEntry entry;
                if(TryProcessRSSItem(item, source, out entry))
                {
                    results.Add(entry);
                }
            }

            if(results.Any())
            {
                entries = results;
                return true;
            }

            return false;
        }

        private bool TryProcessRSSItem(RSSChannelItem item, string source, out AnimeEntry entry)
        {
            entry = default(AnimeEntry);

            FansubFile file;
            OffsetDateTime pubDate;
            string guid;
            Uri resourceLocation;

            if(TryGetFansubFile(item, out file)
                && TryGetPubDate(item, out pubDate)
                && TryGetGuid(item, out guid)
                && TryGetResourceLocation(item, out resourceLocation))
            {
                entry = new AnimeEntry(item.Title, file, pubDate, guid, resourceLocation, source);

                return true;
            }

            return false;
        }

        private bool TryGetFansubFile(RSSChannelItem item, out FansubFile file)
        {
            var fansubFileString = item.Title;
            file = FansubFileParsers.ParseFansubFile(fansubFileString);

            if(string.IsNullOrWhiteSpace(file.SeriesName)
                || string.IsNullOrWhiteSpace(file.FansubGroup)
                || file.EpisodeNumber < 0)
            {
                file = default(FansubFile);
                return false;
            }

            return true;
        }

        private bool TryGetPubDate(RSSChannelItem item, out OffsetDateTime time)
        {
            time = default(OffsetDateTime);

            var pattern = OffsetDateTimePattern.CreateWithInvariantCulture(DateTimePatternRFC822);
            var parseResult = pattern.Parse(item.PublicationDate);

            if(parseResult.Success)
            {
                time = parseResult.Value;

                return true;
            }

            return false;
        }

        private bool TryGetGuid(RSSChannelItem item, out string guid)
        {
            guid = default(string);

            if (string.IsNullOrWhiteSpace(item.Guid))
            {
                return false;
            }

            guid = item.Guid;
            return true;
        }

        private bool TryGetResourceLocation(RSSChannelItem item, out Uri resourceLocation)
        {
            resourceLocation = default(Uri);

            if (string.IsNullOrWhiteSpace(item.Link))
            {
                return false;
            }
            try
            {
                resourceLocation = new Uri(item.Link);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

    }
}
