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
using FansubFileNameParser.Entity;
using FansubFileNameParser.Entity.Directory;
using FansubFileNameParser.Metadata;
using Functional.Maybe;

namespace DataSwallow.Anime
{
    /// <summary>
    /// Represents a search criterion for an Anime Entry
    /// </summary>
    public sealed class AnimeCriterion : ICriterion<AnimeEntry>
    {
        #region private classes
        private sealed class FansubEntityFieldExtractor : IFansubEntityVisitor
        {
            public Maybe<MediaMetadata> Metadata { get; set; }

            public Maybe<string> SeriesName { get; set; }

            public Maybe<string> Group { get; set; }

            public Maybe<string> Extension { get; set; }

            private void SetCoreFields(FansubEntityBase entity)
            {
                Metadata = entity.Metadata;
                Group = entity.Group;
                SeriesName = entity.Series;
            }

            private void SetFileBasedEntity(FansubFileEntityBase entity)
            {
                SetCoreFields(entity);
                Extension = entity.Extension;
            }

            public void Visit(FansubDirectoryEntity entity)
            {
                SetCoreFields(entity);
            }

            public void Visit(FansubMovieEntity entity)
            {
                SetFileBasedEntity(entity);
            }

            public void Visit(FansubOriginalAnimationEntity entity)
            {
                SetFileBasedEntity(entity);
            }

            public void Visit(FansubOPEDEntity entity)
            {
                SetFileBasedEntity(entity);
            }

            public void Visit(FansubEpisodeEntity entity)
            {
                SetFileBasedEntity(entity);
            }
        }
        #endregion

        #region private fields
        private readonly IFansubEntity _fansubEntity;
        private readonly FansubEntityFieldExtractor _extractor;
        #endregion

        #region ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="AnimeCriterion" /> class.
        /// </summary>
        public AnimeCriterion(IFansubEntity fansubEntity)
        {
            _fansubEntity = fansubEntity;
            _extractor = new FansubEntityFieldExtractor();
            _fansubEntity.Accept(_extractor);
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
            return string.Format("Anime Criterion with: {0}", _fansubEntity.ToString());
        }

        /// <summary>
        /// Applies the criterion.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <returns>Returns true if the Anime Entry passes this criterion. False otherwise</returns>
        public bool ApplyCriterion(AnimeEntry entry)
        {
            var entryExtractor = new FansubEntityFieldExtractor();
            entry.FansubEntity.Accept(entryExtractor);

            return _extractor.SeriesName.Equals(entryExtractor.SeriesName)
                && _extractor.Group.Equals(entryExtractor.Group)
                && _extractor.Extension.Equals(entryExtractor.Extension)
                && CompareMetadataQuality(_extractor.Metadata, entryExtractor.Metadata);
        }
        #endregion

        private static bool CompareMetadataQuality(Maybe<MediaMetadata> expected, Maybe<MediaMetadata> candidate)
        {
            if (expected.IsNothing() && candidate.IsNothing())
            {
                return true;
            }

            var result =
                from expectedMedia in expected
                from condidateMedia in candidate
                select
                    expectedMedia.AudioCodec.Equals(condidateMedia.AudioCodec)
                    && expectedMedia.PixelBitDepth.Equals(condidateMedia.PixelBitDepth)
                    && expectedMedia.Resolution.Equals(condidateMedia.Resolution)
                    && expectedMedia.VideoCodec.Equals(condidateMedia.VideoCodec)
                    && expectedMedia.VideoMedia.Equals(condidateMedia.VideoMedia)
                    && expectedMedia.VideoMode.Equals(condidateMedia.VideoMode);

            if (result.IsNothing())
            {
                return false;
            }

            return result.Value;
        }
    }
}
