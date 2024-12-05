using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using HomeStation.Application.CQRS;
using HomeStation.Application.CQRS.ReadingsQuery;
using HomeStation.Controllers;
using HomeStation.Domain.Common.Interfaces;
using Moq;

namespace HomeStation.Web.Tests.Controllers;

[TestFixture]
[TestOf(typeof(AirController))]
[ExcludeFromCodeCoverage]
public class AirControllerTest
{
    private Mock<IQueryDispatcher> queryDispatcher;
    private AirController controller;

    [SetUp]
    public void SetUp()
    {
        queryDispatcher = new Mock<IQueryDispatcher>();
        controller = new AirController(queryDispatcher.Object);
    }

    [Test]
    public async Task GetLatestReadingShouldReturnExpected()
    {
        queryDispatcher.Setup(x =>
                x.Dispatch<GetLatestReadingQuery, ReadingsWebModel>(It.IsAny<GetLatestReadingQuery>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ReadingsWebModel()
            {
                DeviceId = 1,
                Pressure = 1,
                Pm2_5 = 2
            }).Verifiable();

        var expected = new ReadingsWebModel()
        {
            DeviceId = 1,
            Pressure = 1,
            Pm2_5 = 2
        };
        
        ReadingsWebModel result = await controller.GetLatestReading(new GetLatestReadingQuery()
        {
            DeviceId = 1
        });

        result.Should().BeEquivalentTo(expected);
        
        queryDispatcher.Verify(x =>
            x.Dispatch<GetLatestReadingQuery, ReadingsWebModel>(It.IsAny<GetLatestReadingQuery>(),
                It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Test]
    public async Task GetReadingsShouldReturnExpected()
    {
        queryDispatcher.Setup(x =>
                x.Dispatch<GetReadingsQuery, IEnumerable<ReadingsWebModel>>(It.IsAny<GetReadingsQuery>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(new []
            {
                new ReadingsWebModel()
                {
                    DeviceId = 1,
                    Pressure = 1,
                    Pm2_5 = 2
                },
                new ReadingsWebModel()
                {
                    DeviceId = 1,
                    Pressure = 2,
                    Pm1_0 = 1
                }
            }).Verifiable();

        var expected = new List<ReadingsWebModel>()
        {
            new ReadingsWebModel()
            {
                DeviceId = 1,
                Pressure = 1,
                Pm2_5 = 2
            },
            new ReadingsWebModel()
            {
                DeviceId = 1,
                Pressure = 2,
                Pm1_0 = 1
            }
        };
        
        IEnumerable<ReadingsWebModel> result = await controller.GetReadings(new GetReadingsQuery()
        {
            DeviceId = 1
            
        });

        result.Should().BeEquivalentTo(expected);
        
        queryDispatcher.Verify(x =>
            x.Dispatch<GetReadingsQuery, IEnumerable<ReadingsWebModel>>(It.IsAny<GetReadingsQuery>(),
                It.IsAny<CancellationToken>()), Times.Once);
    }
}