using System.Threading.Tasks;
namespace DataSwallow.Stream
{
    /// <summary>
    /// Represents an message sink
    /// </summary>
    /// <typeparam name="TPayload">The type of the payload.</typeparam>
    public interface IOutputMessageSink<TPayload>
    {
        /// <summary>
        /// Accepts the specified message asynchronously.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>A Task representing the accepting of the message</returns>
        Task AcceptAsync(IOutputStreamMessage<TPayload> message);
    }
}
