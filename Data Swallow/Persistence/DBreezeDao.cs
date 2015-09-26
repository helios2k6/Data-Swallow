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
using DBreeze;
using FansubFileNameParser.Entity.Parsers;
using log4net;
using Newtonsoft.Json;
using NodaTime;
using System;
using System.Globalization;

namespace DataSwallow.Persistence
{
    /// <summary>
    /// The DBreeze database DAO for Anime Entries
    /// </summary>
    public sealed class DBreezeDao : IDao<AnimeEntry, string>
    {
        #region nested classes
        /// <summary>
        /// Serializable POD of the AnimeEntry class
        /// </summary>
        [JsonObject(MemberSerialization.OptOut)]
        public sealed class AnimeEntryPOD
        {
            /// <summary>
            /// The original fansub string
            /// </summary>
            [JsonProperty(PropertyName = "OriginalString")]
            public string OriginalString { get; set; }

            /// <summary>
            /// The URI that links to the torrent
            /// </summary>
            [JsonProperty(PropertyName = "Uri")]
            public string Uri { get; set; }

            /// <summary>
            /// The date this entry was published
            /// </summary>
            [JsonProperty(PropertyName = "PublicationDate")]
            public Instant PublicationDate { get; set; }

            /// <summary>
            /// The RSS Source of the entry
            /// </summary>
            [JsonProperty(PropertyName = "Source")]
            public string Source { get; set; }

            /// <summary>
            /// The story's GUID
            /// </summary>
            [JsonProperty(PropertyName = "Guid")]
            public string Guid { get; set; }
        }
        #endregion

        #region private fields
        private static readonly ILog Logger = LogManager.GetLogger(typeof(DBreezeDao));
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
        public void Store(AnimeEntry entry)
        {
            Wrap(() =>
            {
                using (var transaction = _databaseEngine.GetTransaction())
                {
                    transaction.Insert<string, string>(
                        Constants.AnimeEntryTable,
                        entry.Guid,
                        SerializeAnimeEntry(entry)
                    );

                    transaction.Commit();
                }
            });
        }

        /// <summary>
        /// Deletes the specified entry.
        /// </summary>
        /// <param name="entry">The entry.</param>
        public void Delete(AnimeEntry entry)
        {
            Wrap(() =>
            {
                using (var transaction = _databaseEngine.GetTransaction())
                {
                    transaction.RemoveKey<string>(Constants.AnimeEntryTable, entry.Guid);
                    transaction.Commit();
                }
            });
        }

        /// <summary>
        /// Gets the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The result of the Get operation</returns>
        public IDaoResult<AnimeEntry> Get(string key)
        {
            try
            {
                using (var transaction = _databaseEngine.GetTransaction())
                {
                    var row = transaction.Select<string, string>(Constants.AnimeEntryTable, key);
                    if (row.Exists)
                    {
                        return DaoResult<AnimeEntry>.CreateSuccess(DeserializeAnimeEntry(row.Value));
                    }

                    return DaoResult<AnimeEntry>.CreateFailure();
                }
            }
            catch (Exception e)
            {
                Logger.Error(string.Format(CultureInfo.InvariantCulture, "An error occurred while getting the key {0}", key), e);
                return DaoResult<AnimeEntry>.CreateFailure(e);
            }
        }

        /// <summary>
        /// Gets whether or not the 
        /// </summary>
        /// <param name="key">The key</param>
        /// <returns>Whether or not the key exists in the database</returns>
        public bool Contains(string key)
        {
            try
            {
                using (var transaction = _databaseEngine.GetTransaction())
                {
                    return transaction.Select<string, string>(Constants.AnimeEntryTable, key).Exists;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        #region private methods
        private static string SerializeAnimeEntry(AnimeEntry entry)
        {
            return JsonConvert.SerializeObject(Convert(entry));
        }

        private static AnimeEntry DeserializeAnimeEntry(string blob)
        {
            return Convert(JsonConvert.DeserializeObject<AnimeEntryPOD>(blob));
        }

        private static AnimeEntryPOD Convert(AnimeEntry entry)
        {
            return new AnimeEntryPOD
            {
                Guid = entry.Guid,
                OriginalString = entry.OriginalInput,
                PublicationDate = entry.PublicationDate,
                Source = entry.Source,
                Uri = entry.ResourceLocation.ToString(),
            };
        }

        private static AnimeEntry Convert(AnimeEntryPOD pod)
        {
            var animeEntry = EntityParsers.TryParseEntity(pod.OriginalString);
            return new AnimeEntry(pod.OriginalString, animeEntry.Value, pod.PublicationDate, pod.Guid, new Uri(pod.Uri), pod.Source);
        }

        private static void Wrap(Action action)
        {
            try
            {
                action.Invoke();
            }
            catch (Exception e)
            {
                Logger.Error("An error occurred in the DBreezeDao", e);
            }
        }
        #endregion
    }
}
