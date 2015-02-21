
using System;

namespace DataSwallow.Utilities
{
    /// <summary>
    /// Represents a criterion that will always fail 
    /// </summary>
    /// <typeparam name="T">The type of the input</typeparam>
    public sealed class AllFailCriterion<T> : ICriterion<T>
    {
        #region private fields
        /// <summary>
        /// The singleton instance
        /// </summary>
        public static readonly AllFailCriterion<T> Instance = new AllFailCriterion<T>();
        #endregion

        #region ctor
        private AllFailCriterion()
        {
        }
        #endregion

        #region public methods
        /// <summary>
        /// Applies the criterion.
        /// </summary>
        /// <param name="_">The parameter.</param>
        /// <returns>Always false</returns>
        public bool ApplyCriterion(T _)
        {
            return false;
        }
        #endregion
    }
}
