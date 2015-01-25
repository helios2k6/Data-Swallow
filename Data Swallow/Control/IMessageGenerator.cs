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

using DataSwallow.Stream;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataSwallow.Control
{
    /// <summary>
    /// A message generator
    /// </summary>
    /// <typeparam name="TOutput">The type of the output.</typeparam>
    public interface IMessageGenerator<TOutput>
    {
        /// <summary>
        /// Adds an IOutputStream asynchronously
        /// </summary>
        /// <param name="outputStream">The output stream.</param>
        /// <param name="sourcePortNumber">The port number.</param>
        Task AddOutputStreamAsync(IOutputStream<TOutput> outputStream, int sourcePortNumber);
        /// <summary>
        /// Gets the output streams asynchronously.
        /// </summary>
        /// <returns>A Task representing the getting of the output streams</returns>
        Task<IEnumerable<Tuple<IOutputStream<TOutput>, int>>> GetOutputStreamsAsync();
    }
}
