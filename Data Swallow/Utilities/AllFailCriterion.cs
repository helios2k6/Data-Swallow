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
