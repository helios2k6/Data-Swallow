
namespace DataSwallow.Runtime
{
    /// <summary>
    /// Represents the runtime state of an <see cref="ITopologyRuntime"/>
    /// </summary>
    public enum TopologyRuntimeState
    {
        /// <summary>
        /// The not started state
        /// </summary>
        NotStarted,
        /// <summary>
        /// The started state
        /// </summary>
        Started,
        /// <summary>
        /// The paused state
        /// </summary>
        Paused,
        /// <summary>
        /// The stopped state
        /// </summary>
        Stopped,
    }
}
