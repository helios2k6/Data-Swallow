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
        /// <typeparam name="T">Empty type for the input</typeparam>
        /// <typeparam name="L">The empty type for the second input and output</typeparam>
        /// <returns>A function that represents a no-op</returns>
        public static Func<T, L, L> GetNoOp<T, L>()
        {
            return (_, state) => state;
        }


        /// <summary>
        /// Consumes any value, does nothing, and returns nothing
        /// </summary>
        /// <typeparam name="T">The type of input</typeparam>
        /// <param name="t">The input</param>
        public static void Ignore<T>(T t)
        {
        }
    }
}
