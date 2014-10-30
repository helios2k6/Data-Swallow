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

using DataSwallow.Filter.Anime;
using DataSwallow.Source.RSS;
using DataSwallow.Stream;
using NodaTime.Text;
using System;
using System.Threading.Tasks;

namespace DataSwallow
{
    public static class Driver
    {
        private static void Run()
        {
            //var rssSource = new RSSFeedDataSource(new Uri("http://www.nyaa.se/?page=rss"), 3, 0);
            var rssSource = new RSSFeedDataSource(new Uri("http://haruhichan.com/feed/feed.php?mode=rss"), 3, 0);
            var filter = new RSSAnimeDetectionFilter();
            var outputStream = new OutputStream<RSSFeed>(filter, 0);

            var rssContinuation = rssSource.Start();
            var filterContinuation = filter.Start();
            var addingOutputStreamTask = rssSource.AddOutputStreamAsync(outputStream, 0);

            var continuation = Task.WhenAll(rssContinuation, filterContinuation, addingOutputStreamTask);
            continuation.Wait();
        }

        public static void Main(string[] args)
        {
            Run();
        }
    }
}
