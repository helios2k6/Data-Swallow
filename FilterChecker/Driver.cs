﻿/*
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
using DataSwallow.Program.Configuration;
using DataSwallow.Utilities;
using FansubFileNameParser;
using FansubFileNameParser.Metadata;
using Functional.Maybe;
using Newtonsoft.Json;
using NodaTime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FilterChecker
{
    /// <summary>
    /// The main entry point for the program
    /// </summary>
    public static class Driver
    {
        #region public methods
        /// <summary>
        /// The first function to be called in this program
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                PrintHelp();
                return;
            }

            var configurationFile = LoadAnimeConfigurationNames(args[0]);
            var testAnimeNames = args.Skip(1);

            var criterions = configurationFile.Select(CreateGroupCriterion);

            foreach (var testAnimeName in testAnimeNames)
            {
                AnimeEntry testAnimeEntry;
                if (TryCreateAnimeEntry(testAnimeName, out testAnimeEntry))
                {
                    if (criterions.Any(t => t.ApplyCriterion(testAnimeEntry)))
                    {
                        Console.WriteLine(string.Format("{0} PASSED!", testAnimeName));
                    }
                    else
                    {
                        Console.WriteLine(string.Format("{0} FAILED!", testAnimeName));
                    }
                }
            }
        }
        #endregion

        #region private methods
        private static void PrintHelp()
        {
            Console.WriteLine("Filter Checker v1.0");
            Console.WriteLine("Usage: FilterChecker.exe <config file> [test file name 1, test file name 2,...]");
        }

        private static bool TryCreateAnimeEntry(string name, out AnimeEntry animeEntry)
        {
            OffsetDateTime currentTime = SystemClock.Instance.Now.WithOffset(Offset.Zero);

            FansubFile fansubFile;
            MediaMetadata metadata;

            if (FansubFileParsers.TryParseFansubFile(name, out fansubFile) &&
                MediaMetadataParser.TryParseMediaMetadata(name, out metadata))
            {
                animeEntry = new AnimeEntry(name, fansubFile, metadata, currentTime, string.Empty, new Uri("http://nlogneg.com"), string.Empty);
                return true;
            }

            animeEntry = default(AnimeEntry);
            return false;
        }

        private static IEnumerable<string> LoadAnimeConfigurationNames(string filePath)
        {
            return JsonConvert.DeserializeObject<ConfigurationFile>(File.ReadAllText(filePath)).AnimeConfiguration.AnimeReleases;
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
        #endregion
    }
}