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
    /// An anime entry configuration. This object allows the user to specify what anime they want
    /// to download, which fansub group to download it from, and whether to use fuzzy string matching
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class AnimeInfoConfiguration : IEquatable<AnimeInfoConfiguration>
    {
        #region public properties
        /// <summary>
        /// Gets or sets the name of the anime.
        /// </summary>
        /// <value>
        /// The name of the anime.
        /// </value>
        [JsonProperty(Required = Required.Default, PropertyName = "Anime")]
        public string AnimeName { get; set; }

        /// <summary>
        /// Gets or sets the fansub group.
        /// </summary>
        /// <value>
        /// The fansub group.
        /// </value>
        [JsonProperty(Required = Required.Default, PropertyName = "FansubGroup")]
        public string FansubGroup { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use fuzzy string matching.
        /// </summary>
        /// <value>
        ///   <c>true</c> if fuzzy string matching should be used; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty(Required = Required.Default, PropertyName = "UseFuzzy")]
        public bool UseFuzzy { get; set; }
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

            builder.AppendFormat("{{ Anime Name:{0}, Fansub Group: {1}, Use Fuzzy: {2} }}", AnimeName, FansubGroup, UseFuzzy);

            return builder.ToString();
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        public bool Equals(AnimeInfoConfiguration other)
        {
            if (EqualsPreamble(other) == false)
            {
                return false;
            }

            return Equals(AnimeName, other.AnimeName)
                && Equals(FansubGroup, other.FansubGroup)
                && UseFuzzy == other.UseFuzzy;
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
            return Equals(other as AnimeInfoConfiguration);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return AnimeName.GetHashCodeIfNotNull()
                ^ FansubGroup.GetHashCodeIfNotNull()
                ^ UseFuzzy.GetHashCode();
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
