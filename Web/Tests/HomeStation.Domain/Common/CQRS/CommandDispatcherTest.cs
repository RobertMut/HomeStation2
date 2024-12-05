using FluentAssertions;
using HomeStation.Domain.Common.CQRS;
using HomeStation.Domain.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace HomeStation.Domain.Tests.Common.CQRS;

[TestFixture]
[TestOf(typeof(CommandDispatcher))]
public class CommandDispatcherTest
{

    private static int ExecutionCounter = 0;
    [Test]
    public async Task DispatcherShouldExecuteCorrectHandler()
    {
        ServiceCollection collection = new ServiceCollection();
        collection.AddScoped<ICommandHandler<TestCmd>, TestHandler>();

        CommandDispatcher dispatcher = new CommandDispatcher(collection.BuildServiceProvider());

        Func<Task> func = async () => await dispatcher.Dispatch<TestCmd>(new TestCmd(), CancellationToken.None);

        func.Should().NotThrowAsync();
        ExecutionCounter.Should().Be(1);
    }

    private class TestHandler : ICommandHandler<TestCmd>
    {
        public async Task Handle(TestCmd command, CancellationToken cancellationToken, string? clientId = null)
        {
            ExecutionCounter++;
        }
    }

    private class TestCmd : ICommand
    {
        
    }
}