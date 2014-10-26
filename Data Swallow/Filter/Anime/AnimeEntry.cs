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

using FileNameParser;
using NodaTime;
using System;
using System.Text;

namespace DataSwallow.Filter.Anime
{
    /// <summary>
    /// An Anime item entry from some data source
    /// </summary>
    public sealed class AnimeEntry : IEquatable<AnimeEntry>
    {
        #region private fields
        private readonly FansubFile _fansubFile;
        private readonly OffsetDateTime _publicationDate;
        private readonly string _guid;
        private readonly Uri _resourceLocation;
        private readonly string _source;
        #endregion

        #region ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="AnimeEntry" /> class.
        /// </summary>
        /// <param name="fansubFile">The fansub file.</param>
        /// <param name="publicationDate">The publication date.</param>
        /// <param name="guid">The unique identifier.</param>
        /// <param name="resourceLocation">The resource location.</param>
        /// <param name="source">The source.</param>
        public AnimeEntry(FansubFile fansubFile, OffsetDateTime publicationDate, string guid, Uri resourceLocation, string source)
        {
            _fansubFile = fansubFile;
            _publicationDate = publicationDate;
            _guid = guid;
            _resourceLocation = resourceLocation;
            _source = source;
        }
        #endregion

        #region public properties
        /// <summary>
        /// Gets the fansub file.
        /// </summary>
        /// <value>
        /// The fansub file.
        /// </value>
        public FansubFile FansubFile { get { return _fansubFile; } }

        /// <summary>
        /// Gets the publication date.
        /// </summary>
        /// <value>
        /// The publication date.
        /// </value>
        public OffsetDateTime PublicationDate { get { return _publicationDate; } }

        /// <summary>
        /// Gets the unique identifier.
        /// </summary>
        /// <value>
        /// The unique identifier.
        /// </value>
        public string Guid { get { return _guid; } }

        /// <summary>
        /// Gets the resource location.
        /// </summary>
        /// <value>
        /// The resource location.
        /// </value>
        public Uri ResourceLocation { get { return _resourceLocation; } }

        /// <summary>
        /// Gets the source.
        /// </summary>
        /// <value>
        /// The source.
        /// </value>
        public string Source { get { return _source; } }
        #endregion

        #region public methods
        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.AppendLine("Anime Entry with: ");
            builder.AppendFormat("{ File: {0}; Publication Date: {1}; Guid: {2}; URI: {3}", FansubFile, PublicationDate, Guid, ResourceLocation);
            builder.AppendLine();

            return builder.ToString();
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public bool Equals(AnimeEntry other)
        {
            if (EqualsPreamble(other) == false)
            {
                return false;
            }

            return Equals(FansubFile, other.FansubFile)
                && Equals(PublicationDate, other.PublicationDate)
                && Equals(Guid, other.Guid)
                && Equals(ResourceLocation, other.ResourceLocation)
                && Equals(Source, other.Source);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override bool Equals(object other)
        {
            if(EqualsPreamble(other) == false)
            {
                return false;
            }

            return Equals(other as AnimeEntry);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override int GetHashCode()
        {
            return FansubFile.GetHashCode()
                ^ PublicationDate.GetHashCode()
                ^ Guid.GetHashCode()
                ^ ResourceLocation.GetHashCode()
                ^ Source.GetHashCode();
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