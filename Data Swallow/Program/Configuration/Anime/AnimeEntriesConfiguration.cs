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

using DataSwallow.Utilities;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text;

namespace DataSwallow.Program.Configuration.Anime
{
    /// <summary>
    /// Contains the configuration of all anime entries
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class AnimeEntriesConfiguration : IEquatable<AnimeEntriesConfiguration>
    {
        #region public properties
        /// <summary>
        /// Gets or sets the anime releases.
        /// </summary>
        /// <value>
        /// The anime releases.
        /// </value>
        [JsonProperty(Required = Required.Always, PropertyName = "AnimeReleases")]
        public string[] AnimeReleases { get; set; }

        /// <summary>
        /// Gets or sets the RSS feeds.
        /// </summary>
        /// <value>
        /// The RSS feeds.
        /// </value>
        [JsonProperty(Required = Required.Always, PropertyName = "RSSFeeds")]
        public string[] RSSFeeds { get; set; }
        #endregion

        #region ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="AnimeEntriesConfiguration"/> class.
        /// </summary>
        public AnimeEntriesConfiguration()
        {
            AnimeReleases = new string[0];
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
            var builder = new StringBuilder();
            builder.AppendLine("Anime Entries Configuration with entries:");

            foreach (var entry in AnimeReleases)
            {
                builder.AppendLine(entry.ToString());
            }

            return builder.ToString();
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        public bool Equals(AnimeEntriesConfiguration other)
        {
            if (EqualsPreamble(other) == false)
            {
                return false;
            }

            if (AnimeReleases == null)
            {
                return other.AnimeReleases == null;
            }

            return AnimeReleases.SequenceEqual(other.AnimeReleases);
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
            return Equals(other as AnimeEntriesConfiguration);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            if (AnimeReleases == null)
            {
                return 0;
            }

            return AnimeReleases.Aggregate<string, int>(0, (accum, entry) => accum ^ entry.GetHashCodeIfNotNull());
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