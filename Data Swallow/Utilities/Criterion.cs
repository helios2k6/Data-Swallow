namespace DataSwallow.Utilities
{
    /// <summary>
    /// Represents a Criterion
    /// </summary>
    /// <typeparam name="TTarget">The type of the target.</typeparam>
    public sealed class Criterion<TTarget>
    {
        #region private fields
        private readonly TTarget _target;
        private readonly bool _required;
        #endregion

        #region ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="Criterion{TTarget}"/> class.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="required">if set to <c>true</c> [required].</param>
        public Criterion(TTarget target, bool required)
        {
            _target = target;
            _required = required;
        }
        #endregion

        #region public properties
        public TTarget Target { get { return _target; } }

        /// <summary>
        /// Gets a value indicating whether this instance is required.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is required; otherwise, <c>false</c>.
        /// </value>
        public bool IsRequired { get { return _required; } }
        #endregion
    }
}
