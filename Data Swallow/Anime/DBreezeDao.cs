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

using DataSwallow.Utilities;
using DBreeze;
using System;
using System.Threading.Tasks;

namespace DataSwallow.Anime
{
    /// <summary>
    /// The DBreeze database DAO for Anime Entries
    /// </summary>
    public sealed class DBreezeDao : IDao<AnimeEntry, string>
    {
        #region private fields
        private readonly DBreezeEngine _databaseEngine;
        #endregion

        #region ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="DBreezeDao"/> class.
        /// </summary>
        /// <param name="databaseEngine">The database engine.</param>
        public DBreezeDao(DBreezeEngine databaseEngine)
        {
            _databaseEngine = databaseEngine;
        }
        #endregion

        #region public properties
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
            return "DBreeze DAO";
        }

        /// <summary>
        /// Stores the specified entry.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <returns></returns>
        public Task Store(AnimeEntry entry)
        {
            return Task.Factory.StartNew(() =>
            {
                Wrap(() =>
                {
                    using (var transaction = _databaseEngine.GetTransaction())
                    {
                        transaction.Insert<string, AnimeEntry>(Constants.AnimeEntryTable, entry.Guid, entry);
                        transaction.Commit();
                    }
                });
            });
        }

        /// <summary>
        /// Deletes the specified entry.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <returns>A Task representing this operation</returns>
        public Task Delete(AnimeEntry entry)
        {
            return Task.Factory.StartNew(() =>
            {
                Wrap(() =>
                {
                    using (var transaction = _databaseEngine.GetTransaction())
                    {
                        transaction.RemoveKey<string>(Constants.AnimeEntryTable, entry.Guid);
                        transaction.Commit();
                    }
                });
            });
        }

        /// <summary>
        /// Gets the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>A Task representing this operation</returns>
        public Task<IDaoResult<AnimeEntry>> Get(string key)
        {
            return Task.Factory.StartNew<IDaoResult<AnimeEntry>>(() =>
            {
                try
                {
                    using (var transaction = _databaseEngine.GetTransaction())
                    {
                        var row = transaction.Select<string, AnimeEntry>(Constants.AnimeEntryTable, key);
                        if(row.Exists)
                        {
                            return DaoResult<AnimeEntry>.CreateSuccess(row.Value);
                        }

                        return DaoResult<AnimeEntry>.CreateFailure();
                    }
                }
                catch(Exception e)
                {
                    return DaoResult<AnimeEntry>.CreateFailure(e);
                }
            });
        }
        #endregion

        #region private methods
        private void Wrap(Action action)
        {
            try
            {
                action.Invoke();
            }
            catch (Exception)
            {
                //TODO: LOG
            }
        }
        #endregion

    }
}
