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
using DataSwallow.Persistence;
using DataSwallow.Program.Configuration;
using DataSwallow.Program.Configuration.Anime;
using DataSwallow.Runtime;
using DataSwallow.Sink;
using DataSwallow.Source;
using DataSwallow.Source.RSS;
using DataSwallow.Stream;
using DataSwallow.Topology;
using DataSwallow.Utilities;
using DBreeze;
using FansubFileNameParser;
using FansubFileNameParser.Metadata;
using Functional.Maybe;
using log4net;
using log4net.Config;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace DataSwallow.Program
{
    /// <summary>
    /// The bootstrap class for starting this program
    /// </summary>
    public static class Driver
    {
        #region private static fields
        private const int MajorVersion = 1;
        private const int MinorVersion = 5;

        private static int CancelRequests = 0;
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Driver));
        #endregion

        #region public methods
        /// <summary>
        /// The main entry point
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                PrintHelp();
                return;
            }

            try
            {
                ConfigureLogger();
                var configurationFile = LoadConfigurationFile(args[0]);
                using (var dbreezeEngine = new DBreezeEngine(configurationFile.ProgramConfiguration.DatabaseFolder))
                {
                    var runtime = InitializeRuntime(configurationFile, dbreezeEngine);
                    HookupControlCEvent(runtime);
                    StartRuntimeAndWait(runtime);
                }
            }
            catch (Exception e)
            {
                PrintHelp();
                Console.WriteLine("An error occured: " + e.ToString());
                Logger.Fatal("An error occured while configuring the application", e);
            }
        }
        #endregion

        #region private methods
        private static void HookupControlCEvent(ITopologyRuntime runtime)
        {
            Console.CancelKeyPress += new ConsoleCancelEventHandler((sender, arg) =>
            {
                if (Interlocked.Increment(ref CancelRequests) <= 1)
                {
                    runtime.Stop();
                    Logger.Debug("Stop signal received.");
                    Console.WriteLine("Stop signal received. Shutting down cleanly. Hit Ctrl+C again if you want to force shutdown.");
                    arg.Cancel = true;
                }
                else
                {
                    Logger.Debug("Stop signal received multiple times. Force-exiting process! Clean shutdown not achieved!");
                }
            });
        }

        private static void StartRuntimeAndWait(ITopologyRuntime runtime)
        {
            runtime.Start();
            runtime.AwaitTermination();
        }

        private static ITopologyRuntime InitializeRuntime(ConfigurationFile configuration, DBreezeEngine engine)
        {
            return new TopologyRuntime<RSSFeed, AnimeEntry>(CreateTopology(configuration, engine));
        }

        private static void PrintHelp()
        {
            var builder = new StringBuilder();

            builder.AppendFormat("Data Swallow v{0}.{1}", MajorVersion, MinorVersion).AppendLine();
            builder.Append("Usage: <this program> <configuration file>");

            Console.WriteLine(builder.ToString());
        }

        private static void ConfigureLogger()
        {
            XmlConfigurator.Configure(new FileInfo(@"log4net_config.xml"));
        }

        private static ConfigurationFile LoadConfigurationFile(string filePath)
        {
            return JsonConvert.DeserializeObject<ConfigurationFile>(File.ReadAllText(filePath));
        }

        private static ITopology<RSSFeed, AnimeEntry> CreateTopology(ConfigurationFile configuration, DBreezeEngine engine)
        {
            var dataSources = CreateDataSources(configuration.AnimeConfiguration.RSSFeeds);
            var animeFilter = CreateAnimeFilter(configuration.AnimeConfiguration, engine);
            var animeSink = new AnimeEntrySink(configuration.ProgramConfiguration.TorrentFileDestination);

            //Hook up sources to RSS->Anime Entry filter
            foreach (var dataSource in dataSources)
            {
                var outputStream = new OutputStream<RSSFeed>(RSSAnimeDetectionFilter.Instance);
                dataSource.AddOutputStream(outputStream);
            }

            RSSAnimeDetectionFilter.Instance.AddOutputStream(new OutputStream<AnimeEntry>(animeFilter));

            var filtersUpcasted = new List<IFilter>
            {
                animeFilter,
                RSSAnimeDetectionFilter.Instance
            };

            //Hook up sink
            var filterToSink = new OutputStream<AnimeEntry>(animeSink);
            animeFilter.AddOutputStream(filterToSink);

            return new FilterTopology<RSSFeed, AnimeEntry>(dataSources, filtersUpcasted, animeSink.AsEnumerable());
        }

        private static IFilter<AnimeEntry, AnimeEntry> CreateAnimeFilter(AnimeEntriesConfiguration entry, DBreezeEngine engine)
        {
            return new AnimeEntryProcessingFilter(
                new DBreezeDao(engine),
                entry.AnimeReleases.Select(CreateGroupCriterion).ToList());
        }

        private static ICriterion<AnimeEntry> CreateGroupCriterion(string entry)
        {
            FansubFile fansubFile;
            MediaMetadata metadata;

            if (FansubFileParsers.TryParseFansubFile(entry, out fansubFile) &&
                MediaMetadataParser.TryParseMediaMetadata(entry, out metadata))
            {
                var animeCrition = new AnimeCriterion(fansubFile.FansubGroup.ToMaybe(), fansubFile.SeriesName.ToMaybe());

                var qualityCriterion = new QualityCriterion(
                    metadata.VideoMode,
                    metadata.VideoMedia,
                    metadata.Resolution,
                    metadata.AudioCodec,
                    metadata.PixelBitDepth);

                var filePropertyCriterion = new FilePropertyCriterion(fansubFile.Extension.ToMaybe());

                return new GroupCriterion<AnimeEntry>(new ICriterion<AnimeEntry>[] { animeCrition, qualityCriterion, filePropertyCriterion });
            }

            return AllFailCriterion<AnimeEntry>.Instance;
        }

        private static IEnumerable<ISource<RSSFeed>> CreateDataSources(string[] rssFeeds)
        {
            return rssFeeds.Select(t => new RSSFeedDataSource(new Uri(t), 15)).ToList();
        }
        #endregion
    }
}