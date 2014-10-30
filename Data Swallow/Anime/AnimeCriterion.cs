using DataSwallow.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSwallow.Anime
{
    public sealed class AnimeCriterion
    {
        #region private fields
        private readonly Criterion<string> _fansubGroup;
        private readonly Criterion<string> _extension;
        private readonly Criterion<string> _series;
        #endregion

        #region ctor
        public AnimeCriterion(string fansubGroup, string extension, string series)
        {
            _fansubGroup = new Criterion<string>(fansubGroup, true);
            _extension = new Criterion<string>(extension, true);
            _series = new Criterion<string>(series, true);
        }
        #endregion

        #region public methods
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
