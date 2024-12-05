using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using HomeStation.Domain.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace HomeStation.Domain.Tests.Common.CQRS;

[TestFixture]
[TestOf(typeof(QueryDispatcher))]
[ExcludeFromCodeCoverage]
public class QueryDispatcherTest
{
    
    [Test]
    public async Task DispatcherShouldExecuteCorrectHandler()
    {
        ServiceCollection collection = new ServiceCollection();
        collection.AddScoped<IQueryHandler<string, string>, TestHandler>();

        QueryDispatcher dispatcher = new QueryDispatcher(collection.BuildServiceProvider());

        string result = await dispatcher.Dispatch<string, string>("", CancellationToken.None);

        result.Should().BeSameAs("TestHandler");
    }

    private class TestHandler : IQueryHandler<string, string>
    {
        public async Task<string> Handle(string query, CancellationToken cancellationToken)
        {
            return "TestHandler";
        }
    }
}