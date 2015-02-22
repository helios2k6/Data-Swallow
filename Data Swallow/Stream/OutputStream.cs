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

using System.Globalization;

namespace DataSwallow.Stream
{
    /// <summary>
    /// An OutputStream
    /// </summary>
    /// <typeparam name="TOutput">The type of the output.</typeparam>
    public sealed class OutputStream<TOutput> : IOutputStream<TOutput>
    {
        #region private fields
        private readonly IOutputMessageSink<TOutput> _sink;
        #endregion

        #region ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="OutputStream{TOutput}"/> class.
        /// </summary>
        /// <param name="sink">The sink.</param>
        public OutputStream(IOutputMessageSink<TOutput> sink)
        {
            _sink = sink;
        }
        #endregion

        #region public methods
        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "Output Stream for {0}", _sink);
        }

        /// <summary>
        /// Puts the specified output.
        /// </summary>
        /// <param name="output">The output.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void Post(TOutput output)
        {
            var message = new OutputStreamMessage<TOutput>
            {
                Payload = output,
            };

            _sink.Accept(message);
        }
        #endregion
    }
}
