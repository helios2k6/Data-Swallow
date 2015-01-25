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
using Newtonsoft.Json;
using System;
using System.Text;

namespace DataSwallow.Program.Configuration.Anime
{
    /// <summary>
    /// The configuration for an entire anime entry, including what anime to download and all of its
    /// criterions
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class AnimeEntryConfiguration : IEquatable<AnimeEntryConfiguration>
    {
        #region public properties
        /// <summary>
        /// Gets or sets the anime configuration.
        /// </summary>
        /// <value>
        /// The anime configuration.
        /// </value>
        [JsonProperty(Required = Required.Always, PropertyName = "AnimeConfiguration")]
        public AnimeInfoConfiguration AnimeConfiguration { get; set; }

        /// <summary>
        /// Gets or sets the file configuration.
        /// </summary>
        /// <value>
        /// The file configuration.
        /// </value>
        [JsonProperty(Required = Required.Always, PropertyName = "FileConfiguration")]
        public FilePropertyConfiguration FileConfiguration { get; set; }

        /// <summary>
        /// Gets or sets the media configuration.
        /// </summary>
        /// <value>
        /// The media configuration.
        /// </value>
        [JsonProperty(Required = Required.Always, PropertyName = "MediaConfiguration")]
        public MediaQualityConfiguration MediaConfiguration { get; set; }
        #endregion

        #region ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="AnimeEntryConfiguration"/> class.
        /// </summary>
        public AnimeEntryConfiguration()
        {
            AnimeConfiguration = new AnimeInfoConfiguration();
            FileConfiguration = new FilePropertyConfiguration();
            MediaConfiguration = new MediaQualityConfiguration();
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

            builder.AppendLine("Anime Entry Configuration");
            builder.AppendLine(string.Format("Anime Info Config: {0}", AnimeConfiguration));
            builder.AppendLine(string.Format("File Quality Config: {0}", FileConfiguration));
            builder.AppendLine(string.Format("Media Config: {0}", MediaConfiguration));

            return builder.ToString();
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
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        public bool Equals(AnimeEntryConfiguration other)
        {
            if (EqualsPreamble(other) == false)
            {
                return false;
            }

            return Equals(AnimeConfiguration, other.AnimeConfiguration)
                && Equals(FileConfiguration, other.FileConfiguration)
                && Equals(MediaConfiguration, other.MediaConfiguration);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return AnimeConfiguration.GetHashCodeIfNotNull()
                ^ FileConfiguration.GetHashCodeIfNotNull()
                ^ MediaConfiguration.GetHashCodeIfNotNull();
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
