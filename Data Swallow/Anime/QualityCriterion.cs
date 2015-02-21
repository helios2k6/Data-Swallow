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
using FansubFileNameParser.Metadata;
using Functional.Maybe;
using System;
using System.Linq;
using System.Text;

namespace DataSwallow.Anime
{
    /// <summary>
    /// A criterion that encapsulates whether a file passes a certain quality threshold
    /// </summary>
    public sealed class QualityCriterion : ICriterion<AnimeEntry>
    {
        #region private fields
        private readonly Maybe<VideoMode> _videoMode;
        private readonly Maybe<VideoMedia> _videoMedia;
        private readonly Maybe<Resolution> _resolution;
        private readonly Maybe<AudioCodec> _audioCodec;
        private readonly Maybe<PixelBitDepth> _bitDepth;
        #endregion

        #region ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="QualityCriterion"/> class.
        /// </summary>
        /// <param name="videoMode">The video mode.</param>
        /// <param name="videoMedia">The video media.</param>
        /// <param name="resolution">The resolution.</param>
        /// <param name="audioCodec">The audio codec.</param>
        /// <param name="bitDepth">The bit depth.</param>
        public QualityCriterion(
            Maybe<VideoMode> videoMode,
            Maybe<VideoMedia> videoMedia,
            Maybe<Resolution> resolution,
            Maybe<AudioCodec> audioCodec,
            Maybe<PixelBitDepth> bitDepth)
        {
            _videoMode = videoMode;
            _videoMedia = videoMedia;
            _resolution = resolution;
            _audioCodec = audioCodec;
            _bitDepth = bitDepth;
        }
        #endregion

        #region public methods
        /// <summary>
        /// Applies the criterion.
        /// </summary>
        /// <param name="animeEntry">The anime entry to filter.</param>
        /// <returns>Returns true if the AnimeEntry passes this criterion. False otherwise</returns>
        public bool ApplyCriterion(AnimeEntry animeEntry)
        {
            var metadata = animeEntry.MediaMetadata;
            return CheckCriterion(_videoMode, metadata.VideoMode, VideoMode.Unknown)
                && CheckCriterion(_videoMedia, metadata.VideoMedia, VideoMedia.Unknown)
                && CheckCriterion(_resolution, metadata.Resolution, null)
                && CheckCriterion(_audioCodec, metadata.AudioCodec, AudioCodec.Unknown)
                && CheckCriterion(_bitDepth, metadata.PixelBitDepth, PixelBitDepth.Unknown);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendFormat("Quality Criterion with Video Mode {0} and Video Media {1}",
                GetStringForEnum(_videoMode),
                GetStringForEnum(_videoMedia));

            return builder.ToString();
        }
        #endregion

        #region private methods
        private static string GetStringForEnum<T>(Maybe<T> maybe) where T : struct
        {
            return maybe.SelectOrElse(t => Enum.GetName(typeof(T), t), () => Maybe<T>.Nothing.ToString());
        }

        private static bool CheckCriterion<T>(Maybe<T> left, T right, T allClearValue)
        {
            return CheckCriterion(left, right, allClearValue.ToMaybe());
        }

        private static bool CheckCriterion<T>(Maybe<T> left, T right, Maybe<T> allClearValue)
        {
            if (left.IsNothing() && allClearValue.SelectOrElse(t => Equals(t, right), () => false))
            {
                return true;
            }

            return left.SelectOrElse(t => Equals(t, right), () => false);
        }
        #endregion
    }
}