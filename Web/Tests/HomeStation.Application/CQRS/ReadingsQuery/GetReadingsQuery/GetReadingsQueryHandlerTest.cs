using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using HomeStation.Application.Common.Enums;
using HomeStation.Application.Common.Exceptions;
using HomeStation.Application.Common.Interfaces;
using HomeStation.Application.Common.Virtual;
using HomeStation.Application.CQRS;
using HomeStation.Application.CQRS.ReadingsQuery;
using HomeStation.Application.Tests.TestsHelpers;
using HomeStation.Domain.Common.Entities;
using Microsoft.EntityFrameworkCore.Query;
using Moq;

namespace HomeStation.Application.Tests.CQRS.ReadingsQuery;

[TestFixture]
[TestOf(typeof(GetReadingsQueryHandler))]
[ExcludeFromCodeCoverage]
public class GetReadingsQueryHandlerTest
{
    private Mock<Repository<Quality>> qualityRepository;
    private Mock<Repository<Climate>> climateRepository;
    private Mock<Repository<Device>> deviceRepository;

    private IUnitOfWork unitOfWork;
    private GetReadingsQueryHandler handler;
    
    [SetUp]
    public void SetUp()
    {
        qualityRepository = new Mock<Repository<Quality>>(Mock.Of<IAirDbContext>());
        climateRepository = new Mock<Repository<Climate>>(Mock.Of<IAirDbContext>());
        deviceRepository = new Mock<Repository<Device>>(Mock.Of<IAirDbContext>());
        unitOfWork = new UnitOfWorkMock(qualityRepository, climateRepository, deviceRepository).Get;
        
        handler = new GetReadingsQueryHandler(unitOfWork);
    }

    [Test]
    public async Task HandlerShouldReturnExpectedResult()
    {
        DateTimeOffset now = new DateTimeOffset();
        deviceRepository.Setup(x => x.GetObjectBy(It.IsAny<Func<Device?, bool>>(),
                It.IsAny<Func<IQueryable<Device>, IIncludableQueryable<Device, object>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Device()
            {
                Id = 1,
                IsKnown = true,
                AirQuality = new[]
                {
                    new Quality
                    {
                        DeviceId = 1,
                        Pm2_5 = 15,
                        Pm10 = 16,
                        Pm1_0 = 17,
                        Reading = new Reading() { Date = now },
                    }
                },
                Climate = new[]
                {
                    new Climate
                    {
                        DeviceId = 1,
                        Temperature = 4,
                        Humidity = 5,
                        Pressure = 6,
                        Reading = new Reading() { Date = now },
                    }
                }
            });

        List<ReadingsWebModel> expected = new List<ReadingsWebModel>()
        {
            new()
            {
                DeviceId = 1,
                Humidity = 5,
                Pressure = 6,
                Temperature = 4,
                ReadDate = now,
                Pm2_5 = 15,
                Pm1_0 = 17,
                Pm10 = 16
            }
        };
        
        IEnumerable<ReadingsWebModel>? result = await handler.Handle(new GetReadingsQuery()
            {
                DeviceId = 1,
                DetailLevel = DetailLevel.Detailed,
                StartDate = DateTimeOffset.Now.AddDays(-1),
                EndDate = DateTimeOffset.Now.AddDays(1),
                ReadingType = ReadingType.Complete
            },
            CancellationToken.None);

        result.Should().BeEquivalentTo(expected);
    }

    [Test]
    public async Task HandlerShouldReturnTemperatureHumidityOnly()
    {
        DateTimeOffset now = new DateTimeOffset();
        deviceRepository.Setup(x => x.GetObjectBy(It.IsAny<Func<Device?, bool>>(),
                It.IsAny<Func<IQueryable<Device>, IIncludableQueryable<Device, object>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Device()
            {
                Id = 1,
                IsKnown = true,
                Climate = new[]
                {
                    new Climate
                    {
                        DeviceId = 1,
                        Temperature = 4,
                        Humidity = 5,
                        Pressure = 6,
                        Reading = new Reading() { Date = now },
                    }
                }
            });

        List<ReadingsWebModel> expected = new List<ReadingsWebModel>()
        {
            new()
            {
                DeviceId = 1,
                Humidity = 5,
                Pressure = 6,
                Temperature = 4,
                ReadDate = now
            }
        };

        IEnumerable<ReadingsWebModel>? result = await handler.Handle(
            new GetReadingsQuery()
            {
                DeviceId = 1,
                DetailLevel = DetailLevel.Detailed,
                StartDate = DateTimeOffset.Now.AddDays(-1),
                EndDate = DateTimeOffset.Now.AddDays(1),
                ReadingType = ReadingType.Climate
            },
            CancellationToken.None);

        result.Should().BeEquivalentTo(expected);
    }

    [Test] 
    public async Task HandlerShouldReturnAirQualityOnly()
    {
        DateTimeOffset now = new DateTimeOffset();
        deviceRepository.Setup(x => x.GetObjectBy(It.IsAny<Func<Device?, bool>>(),
                It.IsAny<Func<IQueryable<Device>, IIncludableQueryable<Device, object>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Device()
            {
                Id = 1,
                IsKnown = true,
                AirQuality = new[]
                {
                    new Quality
                    {
                        DeviceId = 1,
                        Pm2_5 = 15,
                        Pm10 = 16,
                        Pm1_0 = 17,
                        Reading = new Reading() { Date = now },
                    }
                },
            });

        List<ReadingsWebModel> expected = new List<ReadingsWebModel>()
        {
            new()
            {
                DeviceId = 1,
                Pm2_5 = 15,
                Pm10 = 16,
                Pm1_0 = 17,
                ReadDate = now
            }
        };
        
        IEnumerable<ReadingsWebModel>? result = await handler.Handle(new GetReadingsQuery()
            {
                DeviceId = 1,
                DetailLevel = DetailLevel.Detailed,
                StartDate = DateTimeOffset.Now.AddDays(-1),
                EndDate = DateTimeOffset.Now.AddDays(1),
                ReadingType = ReadingType.Quality
            },
            CancellationToken.None);

        result.Should().BeEquivalentTo(expected);
    }
    
    [Test] 
    public async Task HandlerShouldReturnEmptyUponWrongDate()
    {
        deviceRepository.Setup(x => x.GetObjectBy(It.IsAny<Func<Device?, bool>>(),
                It.IsAny<Func<IQueryable<Device>, IIncludableQueryable<Device, object>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Device()
            {
                Id = 1,
                IsKnown = true,
                AirQuality = Array.Empty<Quality>(),
                Climate = Array.Empty<Climate>()
            });

        List<ReadingsWebModel> expected = new List<ReadingsWebModel>();
        
        IEnumerable<ReadingsWebModel>? result = await handler.Handle(new GetReadingsQuery()
            {
                DeviceId = 1,
                DetailLevel = DetailLevel.Detailed,
                StartDate = DateTimeOffset.Now.AddDays(-100),
                EndDate = DateTimeOffset.Now.AddDays(-99),
                ReadingType = ReadingType.Complete
            },
            CancellationToken.None);

        result.Should().BeEquivalentTo(expected);
    }
    
    [Test] 
    public async Task HandlerShouldThrowDeviceNotFoundException()
    {
        deviceRepository.Setup(x => x.GetObjectBy(It.IsAny<Func<Device?, bool>>(),
                It.IsAny<Func<IQueryable<Device>, IIncludableQueryable<Device, object>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(default(Device));
        
        Func<Task> func = async () => await handler.Handle(new GetReadingsQuery()
            {
                DeviceId = 1,
                DetailLevel = DetailLevel.Detailed,
                ReadingType = ReadingType.Complete
            },
            CancellationToken.None);

        func.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Device not found");
    }
    
    [Test] 
    public async Task HandlerShouldThrowUknownReadingException()
    {
        deviceRepository.Setup(x => x.GetObjectBy(It.IsAny<Func<Device?, bool>>(),
                It.IsAny<Func<IQueryable<Device>, IIncludableQueryable<Device, object>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Device { Id = 1 });
        
        Func<Task> func = async () => await handler.Handle(new GetReadingsQuery()
            {
                DeviceId = 1,
                DetailLevel = DetailLevel.Detailed,
                ReadingType = (ReadingType)15
            },
            CancellationToken.None);

        func.Should().ThrowAsync<UnknownReadingException>()
            .WithMessage("Unknown reading type");
    }
}