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

namespace DataSwallow.Program.Configuration
{
    /// <summary>
    /// The configuration for the media quality that an anime file must meet
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class MediaQualityConfiguration : IEquatable<MediaQualityConfiguration>
    {
        #region public properties
        /// <summary>
        /// Gets or sets the video media.
        /// </summary>
        /// <value>
        /// The video media.
        /// </value>
        [JsonProperty(Required = Required.Default, PropertyName = "VideoMedia")]
        public string VideoMedia { get; set; }

        /// <summary>
        /// Gets or sets the video mode.
        /// </summary>
        /// <value>
        /// The video mode.
        /// </value>
        [JsonProperty(Required = Required.Default, PropertyName = "VideoMode")]
        public string VideoMode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether all settings must match when evaluating anime media files to download.
        /// </summary>
        /// <value>
        ///   <c>true</c> if all criteria must match; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty(Required = Required.Default, PropertyName = "MustMatchAllCriteria")]
        public bool MustMatchAllCriteria { get; set; }
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

            builder.AppendLine("Media Quality Configuration with: ");
            builder.AppendLine(string.Format("Video Media = {0} | Video Mode = {1} | Must Match = {2}", VideoMedia, VideoMode, MustMatchAllCriteria));

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
            return Equals(other as MediaQualityConfiguration);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        public bool Equals(MediaQualityConfiguration other)
        {
            if (EqualsPreamble(other) == false)
            {
                return false;
            }

            return Equals(VideoMedia, other.VideoMedia)
                && Equals(VideoMode, other.VideoMode)
                && MustMatchAllCriteria == other.MustMatchAllCriteria;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return VideoMedia.GetHashCodeIfNotNull()
                ^ VideoMode.GetHashCodeIfNotNull()
                ^ MustMatchAllCriteria.GetHashCode();
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
