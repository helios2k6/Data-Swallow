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
using FansubFileNameParser.Metadata;
using log4net;
using NodaTime;
using NodaTime.Text;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DataSwallow.Filter.Anime
{
    /// <summary>
    /// A RSSFeed -> AnimeEntry processing engine
    /// </summary>
    public static class RSSFeedProcessor
    {
        #region private fields
        private static readonly ILog Logger = LogManager.GetLogger(typeof(RSSFeedProcessor));
        private const string DateTimePatternRFC822 = "ddd, dd MMM yyyy HH:mm:ss o<+HHmm>";
        #endregion

        #region public methods
        /// <summary>
        /// Tries the get anime entries.
        /// </summary>
        /// <param name="feed">The feed.</param>
        /// <param name="entries">The entries.</param>
        /// <returns>True on success. False otherwise</returns>
        public static bool TryGetAnimeEntries(RSSFeed feed, out IEnumerable<AnimeEntry> entries)
        {
            return TryProcessRSSChannel(feed.Channel, out entries);
        }
        #endregion

        #region private methods
        private static bool TryProcessRSSChannel(RSSChannel channel, out IEnumerable<AnimeEntry> entries)
        {
            entries = default(IEnumerable<AnimeEntry>);

            var source = channel.Title;
            var results = new List<AnimeEntry>();

            foreach (var item in channel.Items)
            {
                Logger.DebugFormat("Analyzing RSS Channel Item: {0}", item);
                AnimeEntry entry;
                if (TryProcessRSSItem(item, source, out entry))
                {
                    results.Add(entry);
                }
            }

            if (results.Any())
            {
                entries = results;
                return true;
            }

            return false;
        }

        private static bool TryProcessRSSItem(RSSChannelItem item, string source, out AnimeEntry entry)
        {
            entry = default(AnimeEntry);

            FansubFile file;
            MediaMetadata mediaMetadata;
            OffsetDateTime pubDate;
            string guid;
            Uri resourceLocation;

            if (TryGetFansubFile(item, out file)
                && TryGetMediaMetadata(item, out mediaMetadata)
                && TryGetPubDate(item, out pubDate)
                && TryGetGuid(item, out guid)
                && TryGetResourceLocation(item, out resourceLocation))
            {
                entry = new AnimeEntry(item.Title, file, mediaMetadata, pubDate, guid, resourceLocation, source);

                return true;
            }

            return false;
        }

        private static bool TryGetFansubFile(RSSChannelItem item, out FansubFile file)
        {
            file = default(FansubFile);
            try
            {
                var fansubFileString = item.Title;
                file = FansubFileParsers.ParseFansubFile(fansubFileString);

                if (string.IsNullOrWhiteSpace(file.SeriesName)
                    || string.IsNullOrWhiteSpace(file.FansubGroup)
                    || file.EpisodeNumber < 0)
                {
                    return false;
                }

                return true;
            }
            catch (Exception e)
            {
                Logger.Error(string.Format(CultureInfo.InvariantCulture, "Could not parse RSS Channel Item {0}", item.Title), e);
            }

            return false;
        }

        private static bool TryGetMediaMetadata(RSSChannelItem item, out MediaMetadata mediaMetadata)
        {
            return MediaMetadataParser.TryParseMediaMetadata(item.Title, out mediaMetadata);
        }

        private static bool TryGetPubDate(RSSChannelItem item, out OffsetDateTime time)
        {
            time = default(OffsetDateTime);

            var pattern = OffsetDateTimePattern.CreateWithInvariantCulture(DateTimePatternRFC822);
            var parseResult = pattern.Parse(item.PublicationDate);

            if (parseResult.Success)
            {
                time = parseResult.Value;

                return true;
            }

            return false;
        }

        private static bool TryGetGuid(RSSChannelItem item, out string guid)
        {
            guid = default(string);

            if (string.IsNullOrWhiteSpace(item.Guid))
            {
                return false;
            }

            guid = item.Guid;
            return true;
        }

        private static bool TryGetResourceLocation(RSSChannelItem item, out Uri resourceLocation)
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
