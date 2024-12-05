using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using HomeStation.Application.CQRS.ApproveRevokeDeviceCommand;
using HomeStation.Application.CQRS.GetDevicesQuery;
using HomeStation.Controllers;
using HomeStation.Domain.Common.Interfaces;
using Moq;

namespace HomeStation.Web.Tests.Controllers;

[TestFixture]
[TestOf(typeof(DeviceController))]
[ExcludeFromCodeCoverage]
public class DeviceControllerTest
{

    private Mock<ICommandDispatcher> commandDispatcher;
    private Mock<IQueryDispatcher> queryDispatcher;
    private DeviceController controller;

    [SetUp]
    public void SetUp()
    {
        commandDispatcher = new Mock<ICommandDispatcher>();
        queryDispatcher = new Mock<IQueryDispatcher>();
        controller = new DeviceController(queryDispatcher.Object, commandDispatcher.Object);
    }

    [Test]
    public async Task GetDevicesShouldReturnExpected()
    {
        queryDispatcher.Setup(x =>
            x.Dispatch<GetDevicesQuery, IEnumerable<DeviceWebModel>>(It.IsAny<GetDevicesQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DeviceWebModel[]
            {
                new DeviceWebModel()
                {
                    Id = 1
                }
            }).Verifiable();

        IEnumerable<DeviceWebModel> result = await controller.GetDevices();

        result.Should().BeEquivalentTo(new List<DeviceWebModel>()
        {
            new DeviceWebModel()
            {
                Id = 1
            }
        });
        
        queryDispatcher.Verify(x =>
            x.Dispatch<GetDevicesQuery, IEnumerable<DeviceWebModel>>(It.IsAny<GetDevicesQuery>(),
                It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Test]
    public async Task ApproveShouldReturnExpected()
    {
        commandDispatcher.Setup(x =>
                x.Dispatch(It.IsAny<ApproveRevokeDeviceCommand>(), It.IsAny<CancellationToken>(), It.IsAny<string?>())
                    ).Verifiable();

        Func<Task> task = async () => await controller.Approve(new ApproveRevokeDeviceCommand()
        {
            Id = 1,
            Name = "test"
        });

        task.Should().NotThrowAsync();

        commandDispatcher.Verify(x =>
            x.Dispatch<ApproveRevokeDeviceCommand>(
                It.Is<ApproveRevokeDeviceCommand>(x => x.Id == 1 && x.Name == "test"),
                It.IsAny<CancellationToken>(), It.IsAny<string?>()), Times.Once);
    }
}