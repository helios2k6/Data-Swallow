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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSwallow.Anime
{
    /// <summary>
    /// Represents a search criterion for an Anime Entry
    /// </summary>
    public sealed class AnimeCriterion
    {
        #region private fields
        private readonly Criterion<string> _fansubGroup;
        private readonly Criterion<string> _extension;
        private readonly Criterion<string> _series;
        #endregion

        #region ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="AnimeCriterion"/> class.
        /// </summary>
        /// <param name="fansubGroup">The fansub group.</param>
        /// <param name="extension">The extension.</param>
        /// <param name="series">The series.</param>
        public AnimeCriterion(string fansubGroup, string extension, string series)
        {
            _fansubGroup = new Criterion<string>(fansubGroup, true);
            _extension = new Criterion<string>(extension, true);
            _series = new Criterion<string>(series, true);
        }
        #endregion

        #region public methods
        /// <summary>
        /// Matches the specified entry.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <returns>True if there's a match. False otherwise</returns>
        public bool Match(AnimeEntry entry)
        {
            return ApplyCriterion<string>(_fansubGroup, entry.FansubFile.FansubGroup)
                && ApplyCriterion<string>(_extension, entry.FansubFile.Extension)
                && ApplyCriterion<string>(_series, entry.FansubFile.SeriesName);
        }
        #endregion

        #region private methods
        private bool ApplyCriterion<T>(Criterion<T> criterion, T entry)
        {
            var comparer = EqualityComparer<T>.Default;

            if (comparer.Equals(criterion.Target, entry) || criterion.IsRequired == false)
            {
                return true;
            }

            return false;
        }
        #endregion
            
    }
}
