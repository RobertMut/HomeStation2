using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using HomeStation.Application.Common.Interfaces;
using HomeStation.Application.Common.Virtual;
using HomeStation.Application.CQRS.GetDevicesQuery;
using HomeStation.Application.Tests.TestsHelpers;
using HomeStation.Domain.Common.Entities;
using Moq;

namespace HomeStation.Application.Tests.CQRS.GetDevicesQuery;

[TestFixture]
[TestOf(typeof(GetDevicesQueryHandler))]
[ExcludeFromCodeCoverage]
public class GetDevicesQueryHandlerTest
{

    private IUnitOfWork unitOfWork;
    private GetDevicesQueryHandler handler;
    private Mock<Repository<Device>> deviceRepository;

    [SetUp]
    public void SetUp()
    {
        deviceRepository = new Mock<Repository<Device>>(Mock.Of<IAirDbContext>());
        unitOfWork = new UnitOfWorkMock(deviceRepositoryMock: deviceRepository).Get;
        
        handler = new GetDevicesQueryHandler(unitOfWork);
    }

    [Test]
    public async Task HandlerReturnsExpectedResult()
    {
        deviceRepository.Setup(x => x.GetAll())
            .Returns(new List<Device>()
            {
                new()
                {
                    Id = 1,
                    Name = "first",
                    IsKnown = false,
                },
                new()
                {
                    Id = 2,
                    Name = "second",
                    IsKnown = true,
                },
            }.AsQueryable);

        var expected = new List<DeviceWebModel>()
        {
            new()
            {
                Id = 1,
                Name = "first",
                IsKnown = false
            },
            new()
            {
                Id = 2,
                Name = "second",
                IsKnown = true
            }
        };
        
        IEnumerable<DeviceWebModel> result = await handler.Handle(new Application.CQRS.GetDevicesQuery.GetDevicesQuery(), CancellationToken.None);

        expected.Should().BeEquivalentTo(result);
    } 
}