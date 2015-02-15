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
    /// Extension methods on the base object
    /// </summary>
    public static class BaseExtensions
    {
        /// <summary>
        /// Gets the hash code if not null.
        /// </summary>
        /// <param name="this">This object</param>
        /// <returns>The result of object.GetHashCode() if this is not null. 0 otherwise</returns>
        public static int GetHashCodeIfNotNull(this object @this)
        {
            if (@this != null)
            {
                return @this.GetHashCode();
            }

            return 0;
        }
    }
}
