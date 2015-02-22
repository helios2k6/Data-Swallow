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
using DataSwallow.Control;
using DataSwallow.Stream;
using log4net;
using System;
using System.Globalization;
using System.IO;
using System.Net.Http;

namespace DataSwallow.Sink
{
    /// <summary>
    /// Represents an AnimeEntry Sink
    /// </summary>
    public sealed class AnimeEntrySink : ISink<AnimeEntry>, IDisposable
    {
        #region private fields
        private static readonly ILog Logger = LogManager.GetLogger(typeof(AnimeEntrySink));
        private const string TorrentExtension = ".torrent";

        private readonly string _destinationFolder;
        private readonly FunctionalStatelessActor<AnimeEntry> _engine;
        private readonly HttpClient _client;

        private bool _isDisposed;
        #endregion

        #region ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="AnimeEntrySink"/> class.
        /// </summary>
        /// <param name="destinationEntry">The destination entry.</param>
        public AnimeEntrySink(string destinationEntry)
        {
            _destinationFolder = destinationEntry;
            _engine = new FunctionalStatelessActor<AnimeEntry>(Process);
            _client = new HttpClient();
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
            return "Anime Entry Sink";
        }

        /// <summary>
        /// Starts this instance asynchronously.
        /// </summary>
        /// <returns>A Task representing this action</returns>
        public void Start()
        {
            AssertNotDisposed();

            _engine.Start();
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            AssertNotDisposed();

            _engine.Stop();
        }

        /// <summary>
        /// Blocks the current thread, awaiting for all messages to be processed. Call <see cref="Stop()" /> before calling this.
        /// </summary>
        public void AwaitTermination()
        {
            AssertNotDisposed();

            _engine.AwaitTermination();
        }

        /// <summary>
        /// Accepts messages asynchronously.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public void Accept(IOutputStreamMessage<AnimeEntry> message)
        {
            AssertNotDisposed();

            _engine.Post(message.Payload);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (_isDisposed) return;

            _isDisposed = true;
            _engine.Dispose();
            _client.Dispose();
        }
        #endregion

        #region private methods
        private void AssertNotDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException("AnimeEntrySink");
            }
        }

        private void Process(AnimeEntry entry)
        {
            var torrentFile = _client.GetByteArrayAsync(entry.ResourceLocation).Result;
            var path = Path.Combine(_destinationFolder, entry.OriginalInput + TorrentExtension);

            try
            {
                File.WriteAllBytes(path, torrentFile);
            }
            catch (Exception e)
            {
                Logger.Error(string.Format(CultureInfo.InvariantCulture, "Unable to write torrent file: {0}", path), e);
            }
        }
        #endregion
    }
}
