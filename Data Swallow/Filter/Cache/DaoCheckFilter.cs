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
using System.Collections.Generic;

namespace DataSwallow.Filter.Cache
{
    /// <summary>
    /// A filter that takes an IDao and checks to see if an entry exists in the database before
    /// allowing any AnimeEntry objects to pass
    /// </summary>
    public sealed class DaoCheckFilter : FilterActor<AnimeEntry, AnimeEntry>
    {
        #region private fields
        private readonly IDao<AnimeEntry, string> _dao;
        #endregion

        #region ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="DaoCheckFilter"/> class.
        /// </summary>
        /// <param name="dao">The DAO.</param>
        public DaoCheckFilter(IDao<AnimeEntry, string> dao)
        {
            _dao = dao;
        }
        #endregion

        #region protected methods
        /// <summary>
        /// Digests the message.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="outputStreams">The output streams.</param>
        protected override void DigestMessage(AnimeEntry input, IEnumerable<IOutputStream<AnimeEntry>> outputStreams)
        {
            if (_dao.Get(input.Guid).Success)
            {
                return;
            }

            _dao.Store(input);

            foreach (var outputStream in outputStreams)
            {
                outputStream.Post(input);
            }
        }
        #endregion
    }
}
