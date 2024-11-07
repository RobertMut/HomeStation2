using System.ComponentModel.DataAnnotations;
using HomeStation.Application.Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace HomeStation.Middleware;

/// <summary>
/// The class for handling exceptions
/// </summary>
public class ApiExceptionHandler : IExceptionHandler
{
    private readonly Dictionary<Type, Func<HttpContext, Exception, Task>> _exceptionHandlers;

    /// <summary>
    /// Constructs new ApiExceptionHandler instance.
    /// </summary>
    public ApiExceptionHandler()
    {
        _exceptionHandlers = new()
            {
                { typeof(ValidationException), HandleValidationException },
                { typeof(ApproveException), HandleForbiddenAccessException },
                { typeof(ReadingsException), HandleValidationException },
                { typeof(NotFoundException), HandleNotFoundException },
                { typeof(UnknownReadingException), HandleValidationException },
                { typeof(Exception), HandleUnknownException },
            };
    }

    /// <summary>
    /// Handles exception and invokes designated method
    /// </summary>
    /// <param name="httpContext">The <see cref="HttpContext"/></param>
    /// <param name="exception">The <see cref="Exception"/></param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/></param>
    /// <returns>Returns true if handled</returns>
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var exceptionType = exception.GetType();

        if (_exceptionHandlers.TryGetValue(exceptionType, out var handler))
        {
            await handler.Invoke(httpContext, exception);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Handles validation exception
    /// </summary>
    /// <param name="httpContext">The <see cref="HttpContext"/></param>
    /// <param name="ex">The <see cref="Exception"/></param>
    private async Task HandleValidationException(HttpContext httpContext, Exception ex)
    {
        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails()
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Validation errors.",
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Detail = ex.Message
        });
    }

    /// <summary>
    /// Handles not found exception
    /// </summary>
    /// <param name="httpContext">The <see cref="HttpContext"/></param>
    /// <param name="ex">The <see cref="Exception"/></param>
    private async Task HandleNotFoundException(HttpContext httpContext, Exception ex)
    {
        httpContext.Response.StatusCode = StatusCodes.Status404NotFound;

        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails()
        {
            Status = StatusCodes.Status404NotFound,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            Title = "The specified resource was not found.",
            Detail = ex.Message
        });
    }
    
    /// <summary>
    /// Handles unknown exception
    /// </summary>
    /// <param name="httpContext">The <see cref="HttpContext"/></param>
    /// <param name="ex">The <see cref="Exception"/></param>
    private async Task HandleUnknownException(HttpContext httpContext, Exception ex)
    {
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails()
        {
            Status = StatusCodes.Status500InternalServerError,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            Title = "The specified resource was not found.",
            Detail = ex.Message
        });
    }
    
    /// <summary>
    /// Handles forbidden access exception
    /// </summary>
    /// <param name="httpContext">The <see cref="HttpContext"/></param>
    /// <param name="ex">The <see cref="Exception"/></param>
    private async Task HandleForbiddenAccessException(HttpContext httpContext, Exception ex)
    {
        httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;

        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = StatusCodes.Status403Forbidden,
            Title = "Forbidden",
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3"
        });
    }
}