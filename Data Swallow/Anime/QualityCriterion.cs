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
using System;
using System.Text;

namespace DataSwallow.Anime
{
    /// <summary>
    /// A criterion that encapsulates whether a file passes a certain quality threshold
    /// </summary>
    public sealed class QualityCriterion : ICriterion<AnimeEntry>
    {
        #region private fields
        private readonly VideoMode _videoMode;
        private readonly VideoMedia _videoMedia;
        private bool _allCriteriaMustMatch;
        #endregion

        #region ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="QualityCriterion"/> class.
        /// </summary>
        /// <param name="videoMode">The video mode.</param>
        /// <param name="videoMedia">The video media.</param>
        /// <param name="allCriteriaMustMatch">if set to <c>true</c> allmedia criteria must match.</param>
        public QualityCriterion(VideoMode videoMode, VideoMedia videoMedia, bool allCriteriaMustMatch)
        {
            _videoMode = videoMode;
            _videoMedia = videoMedia;
            _allCriteriaMustMatch = allCriteriaMustMatch;
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
            var mediaCheck = CheckVideoMedia(animeEntry.MediaMetadata.VideoMedia);
            var modeCheck = CheckVideoMode(animeEntry.MediaMetadata.VideoMode);
            if (_allCriteriaMustMatch)
            {
                return mediaCheck && modeCheck;
            }

            return mediaCheck || modeCheck;
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
                Enum.GetName(typeof(VideoMode), _videoMode),
                Enum.GetName(typeof(VideoMedia), _videoMedia));

            return builder.ToString();
        }
        #endregion

        #region private methods
        private bool CheckVideoMode(VideoMode otherVideoMode)
        {
            return _videoMode == VideoMode.Unknown || _videoMode == otherVideoMode;
        }

        private bool CheckVideoMedia(VideoMedia otherVideoMedia)
        {
            return _videoMedia == VideoMedia.Unknown || _videoMedia == otherVideoMedia;
        }
        #endregion
    }
}