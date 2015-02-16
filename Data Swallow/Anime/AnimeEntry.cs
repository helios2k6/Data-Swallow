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

using FansubFileNameParser;
using FansubFileNameParser.Metadata;
using NodaTime;
using System;
using System.Runtime.Serialization;
using System.Text;

namespace DataSwallow.Anime
{
    /// <summary>
    /// An Anime item entry from some data source
    /// </summary>
    [Serializable]
    public sealed class AnimeEntry : IEquatable<AnimeEntry>, ISerializable
    {
        #region private fields
        private const string OriginalInputKey = "OriginalInput";
        private const string FansubFileKey = "FansubFile";
        private const string MediaMetadataKey = "MediaMetadata";
        private const string PublicationDateKey = "PublicationDate";
        private const string GuidKey = "Guid";
        private const string ResourceLocationKey = "ResourceLocation";
        private const string SourceKey = "Source";

        private readonly string _originalInput;
        private readonly FansubFile _fansubFile;
        private readonly MediaMetadata _mediaMetadata;
        private readonly OffsetDateTime _publicationDate;
        private readonly string _guid;
        private readonly Uri _resourceLocation;
        private readonly string _source;
        #endregion

        #region ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="AnimeEntry" /> class.
        /// </summary>
        /// <param name="originalInput">The original input.</param>
        /// <param name="fansubFile">The fansub file.</param>
        /// <param name="mediaMetadata">The media file metadata.</param>
        /// <param name="publicationDate">The publication date.</param>
        /// <param name="guid">The unique identifier.</param>
        /// <param name="resourceLocation">The resource location.</param>
        /// <param name="source">The source.</param>
        public AnimeEntry(
            string originalInput,
            FansubFile fansubFile,
            MediaMetadata mediaMetadata,
            OffsetDateTime publicationDate,
            string guid,
            Uri resourceLocation,
            string source)
        {
            _originalInput = originalInput;
            _fansubFile = fansubFile;
            _mediaMetadata = mediaMetadata;
            _publicationDate = publicationDate;
            _guid = guid;
            _resourceLocation = resourceLocation;
            _source = source;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimeEntry"/> class.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        private AnimeEntry(SerializationInfo info, StreamingContext context)
        {
            _originalInput = (string)info.GetValue(OriginalInputKey, typeof(string));
            _fansubFile = (FansubFile)info.GetValue(FansubFileKey, typeof(FansubFile));
            _mediaMetadata = (MediaMetadata)info.GetValue(MediaMetadataKey, typeof(MediaMetadata));
            _publicationDate = (OffsetDateTime)info.GetValue(PublicationDateKey, typeof(OffsetDateTime));
            _guid = (string)info.GetValue(GuidKey, typeof(string));
            _resourceLocation = (Uri)info.GetValue(ResourceLocationKey, typeof(Uri));
            _source = (string)info.GetValue(SourceKey, typeof(string));
        }
        #endregion

        #region public properties
        /// <summary>
        /// Gets the original input.
        /// </summary>
        /// <value>
        /// The original input.
        /// </value>
        public string OriginalInput { get { return _originalInput; } }

        /// <summary>
        /// Gets the fansub file.
        /// </summary>
        /// <value>
        /// The fansub file.
        /// </value>
        public FansubFile FansubFile { get { return _fansubFile; } }

        /// <summary>
        /// Gets the media metadata.
        /// </summary>
        /// <value>
        /// The media metadata.
        /// </value>
        public MediaMetadata MediaMetadata { get { return _mediaMetadata; } }

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
            builder.AppendLine("File: " + FansubFile.ToString());
            builder.AppendLine("Metadata: " + MediaMetadata.ToString());
            builder.AppendLine("Publication Date: " + PublicationDate.ToString());
            builder.AppendLine("Guid: " + Guid.ToString());
            builder.AppendLine("URL: " + ResourceLocation.ToString());

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

            return Equals(OriginalInput, other.OriginalInput)
                && Equals(FansubFile, other.FansubFile)
                && Equals(MediaMetadata, other.MediaMetadata)
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
            return OriginalInput.GetHashCode()
                ^ FansubFile.GetHashCode()
                ^ MediaMetadata.GetHashCode()
                ^ PublicationDate.GetHashCode()
                ^ Guid.GetHashCode()
                ^ ResourceLocation.GetHashCode()
                ^ Source.GetHashCode();
        }

        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this serialization.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(OriginalInputKey, OriginalInput);
            info.AddValue(FansubFileKey, FansubFile);
            info.AddValue(MediaMetadataKey, MediaMetadata);
            info.AddValue(PublicationDateKey, PublicationDate);
            info.AddValue(GuidKey, Guid);
            info.AddValue(ResourceLocationKey, ResourceLocation);
            info.AddValue(SourceKey, Source);
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
