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
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataSwallow.Topology
{
    /// <summary>
    /// Represents a Topology
    /// </summary>
    /// <typeparam name="TSourceOutput">The type of the source output.</typeparam>
    /// <typeparam name="TFilterInput">The type of the filter input.</typeparam>
    /// <typeparam name="TFilterOutput">The type of the filter output.</typeparam>
    /// <typeparam name="TSinkInput">The type of the sink input.</typeparam>
    public sealed class Topology<TSourceOutput, TFilterInput, TFilterOutput, TSinkInput> : ITopology<TSourceOutput, TFilterInput, TFilterOutput, TSinkInput>
    {
        #region private fields
        private readonly HashSet<ISource<TSourceOutput>> _sources;
        private readonly HashSet<IFilter<TFilterInput, TFilterOutput>> _filters;
        private readonly HashSet<ISink<TSinkInput>> _sinks;
        #endregion

        #region ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="Topology{TSourceOutput, TFilterInput, TFilterOutput, TSinkInput}"/> class as an
        /// empty topology
        /// </summary>
        public Topology()
        {
            _sources = new HashSet<ISource<TSourceOutput>>();
            _filters = new HashSet<IFilter<TFilterInput, TFilterOutput>>();
            _sinks = new HashSet<ISink<TSinkInput>>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Topology{TSourceOutput, TFilterInput, TFilterOutput, TSinkInput}"/> class with
        /// the given sources, filters, and sinks.
        /// </summary>
        /// <param name="sources">The sources.</param>
        /// <param name="filters">The filters.</param>
        /// <param name="sinks">The sinks.</param>
        public Topology(IEnumerable<ISource<TSourceOutput>> sources,
            IEnumerable<IFilter<TFilterInput, TFilterOutput>> filters,
            IEnumerable<ISink<TSinkInput>> sinks)
        {
            _sources = new HashSet<ISource<TSourceOutput>>(sources);
            _filters = new HashSet<IFilter<TFilterInput, TFilterOutput>>(filters);
            _sinks = new HashSet<ISink<TSinkInput>>(sinks);
        }
        #endregion

        #region public methods
        /// <summary>
        /// Gets the sources asynchronous.
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<ISource<TSourceOutput>>> GetSourcesAsync()
        {
            return Task.Factory.StartNew<IEnumerable<ISource<TSourceOutput>>>(() =>
            {
                return _sources;
            });
        }

        /// <summary>
        /// Gets the filters asynchronous.
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<IFilter<TFilterInput, TFilterOutput>>> GetFiltersAsync()
        {
            return Task.Factory.StartNew<IEnumerable<IFilter<TFilterInput, TFilterOutput>>>(() =>
            {
                return _filters;
            });
        }

        /// <summary>
        /// Gets the sinks asynchronous.
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<ISink<TSinkInput>>> GetSinksAsync()
        {
            return Task.Factory.StartNew<IEnumerable<ISink<TSinkInput>>>(() =>
            {
                return _sinks;
            });
        }
        #endregion
    }
}
