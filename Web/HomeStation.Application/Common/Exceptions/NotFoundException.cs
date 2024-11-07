namespace HomeStation.Application.Common.Exceptions;

/// <summary>
/// The not found exception class
/// </summary>
public class NotFoundException : Exception
{
    /// <summary>
    /// Constructs a new instance of the <see cref="NotFoundException"/> class
    /// </summary>
    /// <param name="message">The message string</param>
    public NotFoundException(string message) : base(message)
    {
        
    }
}