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

using System.Collections.Generic;
using System.Linq;

namespace DataSwallow.Utilities
{
    /// <summary>
    /// An extension class for IEnumerable{T}'s
    /// </summary>
    public static class EnumerableExtensions
    {
        #region public methods
        /// <summary>
        /// Calculates the sequence's hash code
        /// </summary>
        /// <typeparam name="TElement">The type of the element.</typeparam>
        /// <param name="this">The IEnumerable{TElement}</param>
        /// <returns>The hash code of this sequence</returns>
        public static int GetSequenceHashCode<TElement>(this IEnumerable<TElement> @this)
        {
            return @this.Aggregate<TElement, int>(0, HashCodeFolder<TElement>);
        }
        #endregion

        #region private methods
        private static int HashCodeFolder<TElement>(int accumulate, TElement element)
        {
            return accumulate ^ element.GetHashCode();
        }
        #endregion
    }
}
