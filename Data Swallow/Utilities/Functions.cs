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

using System;
namespace DataSwallow.Utilities
{
    /// <summary>
    /// A utility class dealing with Functions
    /// </summary>
    public static class Functions
    {
        /// <summary>
        /// Gets the no op action.
        /// </summary>
        /// <typeparam name="T">The type of input</typeparam>
        /// <returns>An Action that does nothing</returns>
        public static Action<T> GetNoOpAction<T>()
        {
            return _ => { };
        }

        /// <summary>
        /// Gets the no op function for two inputs and one output
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="L"></typeparam>
        /// <typeparam name="L"></typeparam>
        /// <returns></returns>
        public static Func<T, L, L> GetNoOp<T, L>()
        {
            return (_, state) => state;
        }
    }
}
