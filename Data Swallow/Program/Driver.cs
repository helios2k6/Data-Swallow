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

using DataSwallow.Anime;
using DataSwallow.Filter;
using DataSwallow.Filter.Anime;
using DataSwallow.Runtime;
using DataSwallow.Sink;
using DataSwallow.Source.RSS;
using DataSwallow.Stream;
using DataSwallow.Topology;
using DBreeze;
using log4net.Config;
using Strilanc.Value;
using System;
using System.IO;

namespace DataSwallow.Program
{
    /// <summary>
    /// The bootstrap class for starting this program
    /// </summary>
    public static class Driver
    {
        private static void Run()
        {
            using (var engine = new DBreezeEngine(@"E:\File Harbor\dbreeze_exp\"))
            {
                var sourceUrl = new Uri("http://www.nyaa.se/?page=rss");
                var dataSource = new RSSFeedDataSource(sourceUrl, 5);

                var rssAnimeFilter = new RSSAnimeDetectionFilter();

                var dbreezeDao = new DBreezeDao(engine);
                var criterion = new AnimeCriterion(May.NoValue, "Kuroko's Basketball", true);
                var animeProcessingFilter = new AnimeEntryProcessingFilter(dbreezeDao, new[] { criterion });

                var sink = new AnimeEntrySink(@"E:\File Harbor\");

                var sourceToFilter = new OutputStream<RSSFeed>(rssAnimeFilter, 0);
                dataSource.AddOutputStreamAsync(sourceToFilter, 0);

                var filterToFilter = new OutputStream<AnimeEntry>(animeProcessingFilter, 0);
                rssAnimeFilter.AddOutputStreamAsync(filterToFilter, 0);

                var filterToSink = new OutputStream<AnimeEntry>(sink, 0);
                animeProcessingFilter.AddOutputStreamAsync(filterToSink, 0);

                var topology = new FilterTopology<RSSFeed, AnimeEntry>(new[] { dataSource }, new IFilter[] { rssAnimeFilter, animeProcessingFilter }, new[] { sink });
                var runtime = new TopologyRuntime<RSSFeed, AnimeEntry>(topology);

                runtime.Start();
                runtime.AwaitTermination(); //Waits forever, never to return
            }
        }

        private static void ConfigureLogger()
        {
            XmlConfigurator.Configure(new FileInfo("log4net_config.xml"));
        }

        /// <summary>
        /// The main entry point
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void Main(string[] args)
        {
            ConfigureLogger();
            Run();
        }
    }
}