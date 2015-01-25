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

using System;

namespace DataSwallow.Persistence
{
    /// <summary>
    /// A query result from an <see cref="IDao{TEntry, TKey}"/>
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public sealed class DaoResult<TResult> : IDaoResult<TResult>
    {
        #region private fields
        private readonly bool _success;
        private readonly TResult _value;
        private readonly Exception _exception;
        #endregion

        #region ctor
        private DaoResult(bool success, TResult value, Exception exception)
        {
            _success = success;
            _value = value;
            _exception = exception;
        }
        #endregion

        #region public properties
        /// <summary>
        /// Gets a value indicating whether this <see cref="DaoResult{TResult}"/> is success.
        /// </summary>
        /// <value>
        ///   <c>true</c> if success; otherwise, <c>false</c>.
        /// </value>
        public bool Success
        {
            get { return _success; }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        /// <exception cref="System.InvalidOperationException">When Success is false</exception>
        public TResult Value
        {
            get 
            { 
                if(_success == false)
                {
                    throw new InvalidOperationException();
                }

                return _value;
            }
        }

        /// <summary>
        /// Gets the error.
        /// </summary>
        /// <value>
        /// The error.
        /// </value>
        public Exception Error
        {
            get { return _exception; }
        }
        #endregion

        #region public methods
        /// <summary>
        /// Creates the success.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A success DAO Result</returns>
        public static DaoResult<TResult> CreateSuccess(TResult value)
        {
            return new DaoResult<TResult>(true, value, default(Exception));
        }

        /// <summary>
        /// Creates the failure.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns>A failure DAO Result</returns>
        public static DaoResult<TResult> CreateFailure(Exception exception)
        {
            return new DaoResult<TResult>(false, default(TResult), exception);
        }

        /// <summary>
        /// Creates a failure DAO Result
        /// </summary>
        /// <returns>A failure DAO Result</returns>
        public static DaoResult<TResult> CreateFailure()
        {
            return CreateFailure(default(Exception));
        }
        #endregion
    }
}
