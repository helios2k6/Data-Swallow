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
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataSwallow.Filter
{
    /// <summary>
    /// Represents a data filter that takes messages and translates them
    /// </summary>
    /// <typeparam name="TInput">The type of the input.</typeparam>
    /// <typeparam name="TOutput">The type of the output.</typeparam>
    public interface IFilter<TInput, TOutput> : IMessageGenerator<TOutput>, IOutputMessageSink<TInput>
    {
        /// <summary>
        /// Processes the message asynchronously.
        /// </summary>
        /// <returns>The Task representing the processing of the message</returns>
        void ProcessAsync();
        /// <summary>
        /// Gets the output streams asynchronously.
        /// </summary>
        /// <returns>A Task representing the getting of IOutputStreams</returns>
        Task<IList<IOutputStream<TOutput>>> GetOutputStreamsAsync();
    }
}
