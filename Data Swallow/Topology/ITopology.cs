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

using DataSwallow.Filter;
using DataSwallow.Sink;
using DataSwallow.Source;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataSwallow.Topology
{
    /// <summary>
    /// Represents the topology of a data processing graph
    /// </summary>
    /// <typeparam name="TSourceOutput">The type of the source output.</typeparam>
    /// <typeparam name="TFilterInput">The type of the filter input.</typeparam>
    /// <typeparam name="TFilterOutput">The type of the filter output.</typeparam>
    /// <typeparam name="TSinkInput">The type of the sink input.</typeparam>
    public interface ITopology<TSourceOutput, TFilterInput, TFilterOutput, TSinkInput>
    {
        /// <summary>
        /// Gets the sources asynchronous.
        /// </summary>
        /// <returns>A Task representing the getting of the ISources</returns>
        Task<IEnumerable<ISource<TSourceOutput>>> GetSourcesAsync();
        /// <summary>
        /// Gets the filters asynchronous.
        /// </summary>
        /// <returns>The Task representing the getting of the IFilters</returns>
        Task<IEnumerable<IFilter<TFilterInput, TFilterOutput>>> GetFiltersAsync();
        /// <summary>
        /// Gets the sinks asynchronous.
        /// </summary>
        /// <returns>The Task representing the getting of the ISinks</returns>
        Task<IEnumerable<ISink<TSinkInput>>> GetSinksAsync();
    }
}
