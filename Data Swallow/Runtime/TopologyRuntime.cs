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
using DataSwallow.Topology;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataSwallow.Runtime
{
    /// <summary>
    /// Represents the runtime of a topology
    /// </summary>
    /// <typeparam name="TSourceOutput">The type of the source output.</typeparam>
    /// <typeparam name="TFilterInput">The type of the filter input.</typeparam>
    /// <typeparam name="TFilterOutput">The type of the filter output.</typeparam>
    /// <typeparam name="TSinkInput">The type of the sink input.</typeparam>
    public sealed class TopologyRuntime<TSourceOutput, TFilterInput, TFilterOutput, TSinkInput> : ITopologyRuntime
    {
        #region private fields
        private readonly ITopology<TSourceOutput, TFilterInput, TFilterOutput, TSinkInput> _topology;
        #endregion

        #region ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="TopologyRuntime{TSourceOutput, TFilterInput, TFilterOutput, TSinkInput}"/> class.
        /// </summary>
        /// <param name="topology">The topology.</param>
        public TopologyRuntime(ITopology<TSourceOutput, TFilterInput, TFilterOutput, TSinkInput> topology)
        {
            _topology = topology;
        }
        #endregion

        #region public properties
        /// <summary>
        /// Gets the running state of the <seealso cref="ITopologyRuntime" />
        /// </summary>
        /// <value>
        /// The running state of the <seealso cref="ITopologyRuntime"/>
        /// </value>
        public TopologyRuntimeState RunningState { get; private set; }

        /// <summary>
        /// Gets the topology.
        /// </summary>
        /// <value>
        /// The topology.
        /// </value>
        public ITopology<TSourceOutput, TFilterInput, TFilterOutput, TSinkInput> Topology 
        { 
            get { return _topology; } 
        }
        #endregion

        #region public methods
        /// <summary>
        /// Starts this instance.
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public void Start()
        {
            if(RunningState != TopologyRuntimeState.NotStarted)
            {
                return;
            }

            RunningState = TopologyRuntimeState.Started;

            /*
             * Must be started in this order:
             * Sink => Filter => Source
             * 
             * So that all messages are received
             */
            MapAllSinks(sink => sink.Start());
            MapAllFilters(filter => filter.Start());
            MapAllSources(source => source.Start());
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public void Stop()
        {
            if(RunningState == TopologyRuntimeState.Stopped)
            {
                return;
            }
            
            RunningState = TopologyRuntimeState.Stopped;

            MapAllSources(source => source.Stop());
            MapAllFilters(filter => filter.Stop());
            MapAllSinks(sink => sink.Stop());
        }

        /// <summary>
        /// Pauses this instance.
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public void Pause()
        {
            if(RunningState != TopologyRuntimeState.Started)
            {
                return;
            }
            
            RunningState = TopologyRuntimeState.Paused;

            MapAllSources(source => source.Pause());
        }

        /// <summary>
        /// Resumes this instance.
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public void Resume()
        {
            if(RunningState != TopologyRuntimeState.Paused)
            {
                return;
            }

            RunningState = TopologyRuntimeState.Started;

            MapAllSources(source => source.Resume());
        }
        #endregion

        #region private methods
        private void MapAllElements<TTarget>(
            Func<ITopology<TSourceOutput, TFilterInput, TFilterOutput, TSinkInput>, IEnumerable<TTarget>> extractor,
            Action<TTarget> actionOnTarget)
        {
            var targets = extractor.Invoke(_topology);
            foreach(var target in targets)
            {
                actionOnTarget.Invoke(target);
            }
        }

        private void MapAllSinks(Action<ISink<TSinkInput>> actionOnSink)
        {
            MapAllElements<ISink<TSinkInput>>(topology => topology.Sinks, actionOnSink);
        }

        private void MapAllFilters(Action<IFilter<TFilterInput, TFilterOutput>> actionOnFilter)
        {
            MapAllElements<IFilter<TFilterInput, TFilterOutput>>(topology => topology.Filters, actionOnFilter);
        }

        private void MapAllSources(Action<ISource<TSourceOutput>> actionOnSource)
        {
            MapAllElements<ISource<TSourceOutput>>(topology => topology.Sources, actionOnSource);
        }
        #endregion

    }
}
