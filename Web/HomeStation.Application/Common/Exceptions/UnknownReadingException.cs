namespace HomeStation.Application.Common.Exceptions;

/// <summary>
/// The unknown readings exception class
/// </summary>
public class UnknownReadingException : ArgumentOutOfRangeException
{
    /// <summary>
    /// Constructs a new instance of the <see cref="ReadingsException"/> class
    /// </summary>
    /// <param name="obj">The object with exception.</param>
    /// <param name="message">The message string.</param>
    public UnknownReadingException(object obj, string? message) : base(nameof(obj), message)
    {
    }
}