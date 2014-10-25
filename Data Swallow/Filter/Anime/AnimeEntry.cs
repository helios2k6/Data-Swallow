
using FileNameParser;
using NodaTime;
using System;

namespace DataSwallow.Filter.Anime
{
    /// <summary>
    /// An Anime item entry from some data source
    /// </summary>
    public sealed class AnimeEntry : IEquatable<AnimeEntry>
    {
        #region private fields
        private readonly FansubFile _fansubFile;
        private readonly ZonedDateTime _publicationDate;
        private readonly string _guid;
        private readonly Uri _resourceLocation;
        #endregion

        #region ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="AnimeEntry"/> class.
        /// </summary>
        /// <param name="fansubFile">The fansub file.</param>
        /// <param name="publicationDate">The publication date.</param>
        /// <param name="guid">The unique identifier.</param>
        /// <param name="resourceLocation">The resource location.</param>
        public AnimeEntry(FansubFile fansubFile, ZonedDateTime publicationDate, string guid, Uri resourceLocation)
        {
            _fansubFile = fansubFile;
            _publicationDate = publicationDate;
            _guid = guid;
            _resourceLocation = resourceLocation;
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
        public ZonedDateTime PublicationDate { get { return _publicationDate; } }

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
            throw new System.NotImplementedException();
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
                && Equals(ResourceLocation, other.ResourceLocation);
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
                ^ ResourceLocation.GetHashCode();
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
