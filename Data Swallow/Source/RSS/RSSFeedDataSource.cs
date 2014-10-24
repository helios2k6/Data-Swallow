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

namespace DataSwallow.Source.RSS
{
    /// <summary>
    /// Represents a Data Source based off a RSS Feed
    /// </summary>
    public sealed class RSSFeedDataSource : ISource<RSSFeed>
    {
        private readonly Uri _feedUrl;
        private readonly int _pauseTime;
        private readonly int _variability;

        /// <summary>
        /// Initializes a new instance of the <see cref="RSSFeedDataSource"/> class.
        /// </summary>
        /// <param name="feedUrl">The feed URL.</param>
        /// <param name="pauseTime">The pause time.</param>
        /// <param name="variability">The variability.</param>
        public RSSFeedDataSource(Uri feedUrl, int pauseTime, int variability)
        {
            _feedUrl = feedUrl;
            _pauseTime = pauseTime;
            _variability = variability;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RSSFeedDataSource"/> class.
        /// </summary>
        /// <param name="feedUrl">The feed URL.</param>
        /// <param name="pauseTime">The pause time.</param>
        public RSSFeedDataSource(Uri feedUrl, int pauseTime)
            : this(feedUrl, pauseTime, 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RSSFeedDataSource"/> class.
        /// </summary>
        /// <param name="feedUrl">The feed URL.</param>
        public RSSFeedDataSource(Uri feedUrl)
            : this(feedUrl, 60, 0)
        {
        }

        /// <summary>
        /// Gets the output streams asynchronously.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Task<IList<IOutputStream<RSSFeed>>> GetOutputStreamsAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds the output stream asynchronously.
        /// </summary>
        /// <param name="outputStream">The output stream.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Task AddOutputStreamAsync(IOutputStream<RSSFeed> outputStream)
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public void Pause()
        {
            throw new NotImplementedException();
        }

        public void AddOutputStream(IOutputStream<RSSFeed> outputStream)
        {
            throw new NotImplementedException();
        }
    }
}
