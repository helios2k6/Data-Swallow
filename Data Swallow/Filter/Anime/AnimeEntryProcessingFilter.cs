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
using DataSwallow.Persistence;
using DataSwallow.Stream;
using DataSwallow.Utilities;
using log4net;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataSwallow.Filter.Anime
{
    /// <summary>
    /// Processes an Anime Entry
    /// </summary>
    public sealed class AnimeEntryProcessingFilter : FilterActor<AnimeEntry, AnimeEntry>
    {
        #region private fields
        private static readonly ILog Logger = LogManager.GetLogger(typeof(AnimeEntryProcessingFilter));

        private readonly IDao<AnimeEntry, string> _dao;
        private readonly IEnumerable<ICriterion<AnimeEntry>> _criterions;
        private readonly bool _allCriterionsMustPass;
        #endregion

        #region ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="AnimeEntryProcessingFilter" /> class.
        /// </summary>
        /// <param name="dao">The DAO.</param>
        /// <param name="criterions">The criterions.</param>
        /// <param name="allCriterionsMustPass">Whether all criterions must pass in order for this filter to accept an AnimeEntry</param>
        public AnimeEntryProcessingFilter(IDao<AnimeEntry, string> dao, IEnumerable<ICriterion<AnimeEntry>> criterions, bool allCriterionsMustPass)
        {
            _dao = dao;
            _criterions = criterions;
            _allCriterionsMustPass = allCriterionsMustPass;
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
        /// <param name="input">The input.</param>
        /// <param name="outputStreams">The output streams.</param>
        protected override void DigestMessage(AnimeEntry input,IEnumerable<IOutputStream<AnimeEntry>> outputStreams)
        {
            DigestMessageImpl(input, outputStreams).Wait();
        }
        #endregion

        #region private methods
        private async Task DigestMessageImpl(AnimeEntry entry, IEnumerable<IOutputStream<AnimeEntry>> outputStreams)
        {
            //Check to see if the entry exists
            var doesEntryExist = await DoesEntryAlreadyExist(entry);
            if (doesEntryExist)
            {
                return;
            }

            Logger.DebugFormat("Received Anime Entry: {0}", entry);

            //Add the entry to the database
            await _dao.Store(entry);

            //See if it matches any criterion
            if (DoesPassCriterions(entry))
            {
                foreach (var outputStream in outputStreams)
                {
                    Logger.DebugFormat("Accepting Anime Entry: {0}", entry);
                    outputStream.Post(entry);
                }
            }
        }

        private async Task<bool> DoesEntryAlreadyExist(AnimeEntry entry)
        {
            var existingEntry = await _dao.Get(entry.Guid);

            return existingEntry.Success;
        }

        private bool DoesPassCriterions(AnimeEntry entry)
        {
            if (_allCriterionsMustPass)
            {
                return _criterions.All(t => t.ApplyCriterion(entry));
            }

            return _criterions.Any(t => t.ApplyCriterion(entry));
        }
        #endregion
    }
}
