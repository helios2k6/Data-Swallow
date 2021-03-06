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
using DataSwallow.Persistence;
using DataSwallow.Stream;
using DataSwallow.Utilities;
using log4net;
using System.Collections.Generic;
using System.Linq;

namespace DataSwallow.Filter.Anime
{
    /// <summary>
    /// Processes an Anime Entry
    /// </summary>
    public sealed class AnimeEntryProcessingFilter : FilterActor<AnimeEntry, AnimeEntry>
    {
        #region private fields
        private static readonly ILog Logger = LogManager.GetLogger(typeof(AnimeEntryProcessingFilter));

        private readonly IEnumerable<ICriterion<AnimeEntry>> _criterions;
        #endregion

        #region ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="AnimeEntryProcessingFilter" /> class.
        /// </summary>
        /// <param name="criterions">The criterions.</param>
        public AnimeEntryProcessingFilter(IEnumerable<ICriterion<AnimeEntry>> criterions)
        {
            _criterions = criterions;
        }
        #endregion

        #region public methods
        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return "Anime Entry Processing Filter";
        }
        #endregion

        #region protected methods
        /// <summary>
        /// Digests the message.
        /// </summary>
        /// <param name="entry">The AnimeEntry.</param>
        /// <param name="outputStreams">The output streams.</param>
        protected override void DigestMessage(AnimeEntry entry, IEnumerable<IOutputStream<AnimeEntry>> outputStreams)
        {
            //See if it matches any criterion
            if (DoesPassCriterions(entry))
            {
                Logger.DebugFormat("Received Anime Entry: {0}", entry);
                foreach (var outputStream in outputStreams)
                {
                    outputStream.Post(entry);
                }
            }
        }
        #endregion

        #region private methods
        private bool DoesPassCriterions(AnimeEntry entry)
        {
            return _criterions.Any(t => t.ApplyCriterion(entry));
        }
        #endregion
    }
}
