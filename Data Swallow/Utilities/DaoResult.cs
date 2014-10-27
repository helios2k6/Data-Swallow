
using System;

namespace DataSwallow.Utilities
{
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
