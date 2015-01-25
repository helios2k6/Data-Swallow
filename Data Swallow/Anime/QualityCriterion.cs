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
    public sealed class QualityCriterion : ICriterion<MediaMetadata>
    {
        #region private fields
        private readonly VideoMode _videoMode;
        private readonly VideoMedia _videoMedia;
        private bool _bothMustMatch;
        #endregion

        #region ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="QualityCriterion"/> class.
        /// </summary>
        /// <param name="videoMode">The video mode.</param>
        /// <param name="videoMedia">The video media.</param>
        /// <param name="bothMustMatch">if set to <c>true</c> [both must match].</param>
        public QualityCriterion(VideoMode videoMode, VideoMedia videoMedia, bool bothMustMatch)
        {
            _videoMode = videoMode;
            _videoMedia = videoMedia;
            _bothMustMatch = bothMustMatch;
        }
        #endregion

        #region public methods
        /// <summary>
        /// Applies the criterion.
        /// </summary>
        /// <param name="mediaMetadata">The media metadata.</param>
        /// <returns>Returns true if the MediaMetadata passes this criterion. False otherwise</returns>
        public bool ApplyCriterion(MediaMetadata mediaMetadata)
        {
            if (_bothMustMatch)
            {
                return _videoMode == mediaMetadata.VideoMode && _videoMedia == mediaMetadata.VideoMedia;
            }

            return _videoMode == mediaMetadata.VideoMode || _videoMedia == mediaMetadata.VideoMedia;
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
    }
}