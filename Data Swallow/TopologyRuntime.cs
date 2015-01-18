using DataSwallow.Topology;
namespace DataSwallow
{
    /// <summary>
    /// Represents the runtime of a topology
    /// </summary>
    /// <typeparam name="TSourceOutput">The type of the source output.</typeparam>
    /// <typeparam name="TFilterInput">The type of the filter input.</typeparam>
    /// <typeparam name="TFilterOutput">The type of the filter output.</typeparam>
    /// <typeparam name="TSinkInput">The type of the sink input.</typeparam>
    public sealed class TopologyRuntime<TSourceOutput, TFilterInput, TFilterOutput, TSinkInput>
    {
        #region private fields
        private readonly ITopology<TSourceOutput, TFilterInput, TFilterOutput, TSinkInput> _topology;
        #endregion

        #region ctor
        public TopologyRuntime(ITopology<TSourceOutput, TFilterInput, TFilterOutput, TSinkInput> topology)
        {
            _topology = topology;
        }
        #endregion

        #region public properties
        public bool IsStarted { get; private set; }
        public bool IsStopped { get; private set; }
        public bool IsPaused { get; private set; }

        /// <summary>
        /// Gets the topology.
        /// </summary>
        /// <value>
        /// The topology.
        /// </value>
        public ITopology<TSourceOutput, TFilterInput, TFilterOutput, TSinkInput> Topology 
        { 
            get { return _topology; } 
        }
        #endregion

        #region public methods
        public void Start()
        {
        }

        public void Stop()
        {
        }

        public void Pause()
        {
        }

        public void Resume()
        {
        }
        #endregion

        #region private methods
        #endregion
            
    }
}
