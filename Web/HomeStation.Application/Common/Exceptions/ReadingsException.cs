namespace HomeStation.Application.Common.Exceptions;

/// <summary>
/// The readings exception class
/// </summary>
public class ReadingsException : Exception
{
    /// <summary>
    /// Constructs a new instance of the <see cref="ReadingsException"/> class
    /// </summary>
    /// <param name="message">The message string</param>
    public ReadingsException(string? message) : base(message)
    {
    }
}