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
using Functional.Maybe;
using System;

namespace DataSwallow.Anime
{
    /// <summary>
    /// Represents a search criterion for an Anime Entry
    /// </summary>
    public sealed class AnimeCriterion : ICriterion<AnimeEntry>
    {
        #region private fields
        private readonly Maybe<string> _fansubGroup;
        private readonly Maybe<string> _series;
        #endregion

        #region ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="AnimeCriterion" /> class.
        /// </summary>
        /// <param name="fansubGroup">The fansub group.</param>
        /// <param name="series">The series.</param>
        public AnimeCriterion(Maybe<string> fansubGroup, Maybe<string> series)
        {
            _fansubGroup = fansubGroup;
            _series = series;
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
            return string.Format("Anime Criterion with [{0}] {1}",
                _fansubGroup.ReturnToStringAuto(),
                _series.ReturnToStringAuto());
        }

        /// <summary>
        /// Applies the criterion.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <returns>Returns true if the Anime Entry passes this criterion. False otherwise</returns>
        public bool ApplyCriterion(AnimeEntry entry)
        {
            return ApplyCriterion(_fansubGroup, entry.FansubFile.FansubGroup)
                && ApplyCriterion(_series, entry.FansubFile.SeriesName);
        }
        #endregion

        #region private methods
        private bool ApplyCriterion(Maybe<string> criterion, string entry)
        {
            return criterion.SelectOrElse(
                crit => crit.Equals(entry, StringComparison.Ordinal),
                () => true);
        }
        #endregion
    }
}
