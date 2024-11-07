namespace HomeStation.Application.Common.Exceptions;

/// <summary>
/// The approve exception class
/// </summary>
public class ApproveException : Exception
{
    /// <summary>
    /// Constructs a new instance of the <see cref="ApproveException"/> class
    /// </summary>
    /// <param name="message">The message string</param>
    public ApproveException(string message) 
        : base($"Failed to approve or disapprove device. Reason: {message}.")
    {
        
    }
}