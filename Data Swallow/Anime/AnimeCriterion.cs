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
using SimMetricsApi;
using SimMetricsMetricUtilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataSwallow.Anime
{
    /// <summary>
    /// Represents a search criterion for an Anime Entry
    /// </summary>
    public sealed class AnimeCriterion : ICriterion<AnimeEntry>
    {
        #region private classes
        private sealed class StringMetricsCalculator
        {
            private static readonly Lazy<StringMetricsCalculator> _instanceLazy = new Lazy<StringMetricsCalculator>(() => new StringMetricsCalculator());

            public static StringMetricsCalculator Instance
            {
                get { return _instanceLazy.Value; }
            }

            private IList<AbstractStringMetric> CreateMetricsList()
            {
                return new List<AbstractStringMetric>
                {
                    new JaroWinkler(),
                    new Levenstein(),
                    new MongeElkan(),
                    new NeedlemanWunch(),
                    new QGramsDistance(),
                    new SmithWatermanGotoh(),
                };
            }

            private double GetSimilatiry(string a, string b)
            {
                return CreateMetricsList().Average(c => c.GetSimilarity(a, b));
            }

            public double MeasureSimilarity(string a, string b)
            {
                return GetSimilatiry(a, b);
            }

            public double MeasureSimilarityIgnoreCase(string a, string b)
            {
                string aUpper = a.ToUpperInvariant();
                string bUpper = b.ToUpperInvariant();

                return MeasureSimilarity(aUpper, bUpper);
            }
        }
        #endregion

        #region private fields
        private const double SmudgeFactor = 0.80;
        private const double Epsilon = 0.000001;

        private readonly Maybe<string> _fansubGroup;
        private readonly Maybe<string> _series;
        private readonly bool _useFuzzyMatch;
        #endregion

        #region ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="AnimeCriterion" /> class.
        /// </summary>
        /// <param name="fansubGroup">The fansub group.</param>
        /// <param name="series">The series.</param>
        /// <param name="useFuzzyMatch">Whether or not to use string distances to match</param>
        public AnimeCriterion(Maybe<string> fansubGroup, Maybe<string> series, bool useFuzzyMatch)
        {
            _fansubGroup = fansubGroup;
            _series = series;
            _useFuzzyMatch = useFuzzyMatch;
        }
        #endregion

        #region public methods
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
            if (_useFuzzyMatch)
            {
                return ApplyCriterionFuzzy(criterion, entry);
            }
            else
            {
                return ApplyCriterionExact(criterion, entry);
            }
        }

        private bool ApplyCriterionExact(Maybe<string> criterion, string entry)
        {
            return criterion.SelectOrElse(
                crit => crit.Equals(entry, StringComparison.Ordinal), 
                () => true);
        }

        private bool ApplyCriterionFuzzy(Maybe<string> criterion, string entry)
        {
            return criterion.SelectOrElse(
                crit => ApplyCriterionFuzzyImpl(crit, entry), 
                () => true);
        }

        private bool ApplyCriterionFuzzyImpl(string criterion, string entry)
        {
            var distance = StringMetricsCalculator.Instance.MeasureSimilarityIgnoreCase(criterion, entry);

            var comparison = distance - SmudgeFactor;
            var comparisonMagnitude = Math.Abs(comparison);

            if (comparison >= 0.0 && comparisonMagnitude >= Epsilon)
            {
                return true;
            }

            return false;
        }
        #endregion
    }
}
