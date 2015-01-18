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
using FansubFileNameParser.Metadata;

namespace DataSwallow.Anime
{
    /// <summary>
    /// A criterion that encapsulates whether a file matches the specific metafile qualities desired
    /// </summary>
    public sealed class FilePropertyCriterion : ICriterion<AnimeEntry>
    {
        #region private fields
        private readonly string _extension;
        #endregion

        #region ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="FilePropertyCriterion"/> class.
        /// </summary>
        /// <param name="extension">The extension.</param>
        public FilePropertyCriterion(string extension)
        {
            _extension = extension;
        }
        #endregion

        #region public methods
        /// <summary>
        /// Applies the criterion.
        /// </summary>
        /// <param name="animeEntry">The <seealso cref="AnimeEntry"/></param>
        /// <returns>True if the criterion is passed. False otherwise</returns>
        public bool ApplyCriterion(AnimeEntry animeEntry)
        {
            return animeEntry.FansubFile.Extension.Equals(_extension, System.StringComparison.OrdinalIgnoreCase);
        }
        #endregion
    }
}
