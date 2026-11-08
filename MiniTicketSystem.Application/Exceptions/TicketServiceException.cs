namespace MiniTicketSystem.Application.Exceptions
{
    /// <summary>
    /// Represents an error that occurred while processing a ticket operation. The message is safe to return to the client.
    /// </summary>
    public class TicketServiceException : Exception
    {
        public TicketServiceException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
