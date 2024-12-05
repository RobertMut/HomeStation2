using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using HomeStation.Application.Common.Enums;
using HomeStation.Application.Common.Interfaces;
using HomeStation.Application.Common.Virtual;
using HomeStation.Application.CQRS.ApproveRevokeDeviceCommand;
using HomeStation.Application.Tests.TestsHelpers;
using HomeStation.Domain.Common.Entities;
using Microsoft.EntityFrameworkCore.Query;
using Moq;

namespace HomeStation.Application.Tests.CQRS.ApproveRevokeDeviceCommand;

[TestFixture]
[TestOf(typeof(ApproveRevokeDeviceCommandHandler))]
[ExcludeFromCodeCoverage]
public class ApproveRevokeDeviceCommandHandlerTest
{
    private IUnitOfWork unitOfWork;
    private ApproveRevokeDeviceCommandHandler handler;
    private Mock<Repository<Device>> deviceRepository;

    [SetUp]
    public void SetUp()
    {
        deviceRepository = new Mock<Repository<Device>>(Mock.Of<IAirDbContext>());
        unitOfWork = new UnitOfWorkMock(deviceRepositoryMock: deviceRepository).Get;
        
        handler = new ApproveRevokeDeviceCommandHandler(unitOfWork);
    }

    [Test]
    public async Task HandlerReturnsExpectedResult()
    {
        var device = new Device
        {
            Id = 5,
            Name = "Device",
            IsKnown = false,
        };
        
        deviceRepository.Setup(x => x.GetObjectBy(It.IsAny<Func<Device?, bool>>(),
                It.IsAny<Func<IQueryable<Device>, IIncludableQueryable<Device, object>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(device);
        deviceRepository.Setup(x => x.UpdateAsync(It.IsAny<Device?>()));
        
        Func<Task> task = async () => await handler.Handle(new Application.CQRS.ApproveRevokeDeviceCommand.ApproveRevokeDeviceCommand()
        {
            Id = 5,
            Name = "Device",
            Operation = OperationType.Approve
        }, CancellationToken.None);

        task.Should().NotThrowAsync();
        
        deviceRepository.Setup(x =>
                x.UpdateAsync(It.Is<Device?>(d => d.IsKnown && d.Id == 5 && d.Name == "Device")))
            .Verifiable();
    } 
}