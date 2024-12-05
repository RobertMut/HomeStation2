using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using HomeStation.Application.Common.Exceptions;
using HomeStation.Middleware;
using Microsoft.AspNetCore.Http;

namespace HomeStation.Web.Tests.Middleware;

[TestFixture]
[TestOf(typeof(ApiExceptionHandler))]
[ExcludeFromCodeCoverage]
public class ApiExceptionHandlerTest
{
    private ApiExceptionHandler exceptionHandler;
    private const string Message = "Message";
    private static IEnumerable TestCases
    {
        get
        {
            yield return new TestCaseData(new ValidationException(Message)).Returns(StatusCodes.Status400BadRequest);
            yield return new TestCaseData(new ApproveException(Message)).Returns(StatusCodes.Status403Forbidden);
            yield return new TestCaseData(new ReadingsException(Message)).Returns(StatusCodes.Status400BadRequest);
            yield return new TestCaseData(new NotFoundException(Message)).Returns(StatusCodes.Status404NotFound);
            yield return new TestCaseData(new UnknownReadingException(new object(), Message)).Returns(StatusCodes.Status400BadRequest);
            yield return new TestCaseData(new Exception(Message)).Returns(StatusCodes.Status500InternalServerError);
        }
    }
    
    [SetUp]
    public void SetUp()
    {
        exceptionHandler = new ApiExceptionHandler();
    }
    
    [TestCaseSource(nameof(TestCases))]
    public async Task<int> ValidateException(Exception exception)
    {
        var context = new DefaultHttpContext()
        {
            Response = {  },
        };
        
        bool result = await exceptionHandler.TryHandleAsync(context, exception, CancellationToken.None);
        
        result.Should().BeTrue();

        return context.Response.StatusCode;
    }
}