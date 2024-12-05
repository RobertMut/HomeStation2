using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
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
[TestOf(typeof(GetLatestReadingQueryHandler))]
[ExcludeFromCodeCoverage]
public class GetLatestReadingQueryHandlerTest
{
    private Mock<Repository<Quality>> qualityRepository;
    private Mock<Repository<Climate>> climateRepository;

    private IUnitOfWork unitOfWork;
    private GetLatestReadingQueryHandler handler;

    [SetUp]
    public void SetUp()
    {
        qualityRepository = new Mock<Repository<Quality>>(Mock.Of<IAirDbContext>());
        climateRepository = new Mock<Repository<Climate>>(Mock.Of<IAirDbContext>());

        unitOfWork = new UnitOfWorkMock(qualityRepository, climateRepository).Get;
        
        handler = new GetLatestReadingQueryHandler(unitOfWork);
    }
    
    [Test]
    public async Task HandlerReturnsExpectedResult()
    {
        DateTimeOffset now = DateTimeOffset.Now;

        qualityRepository.Setup(x => x.GetLastBy(It.IsAny<Func<Quality?, bool>>(),
                It.IsAny<Func<IQueryable<Quality>, IIncludableQueryable<Quality, object>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Quality()
            {
                DeviceId = 1,
                Pm1_0 = 1,
                Pm2_5 = 1,
                Pm10 = 1,
                Reading = new Reading() { Date = now }
            });
        climateRepository.Setup(x => x.GetLastBy(It.IsAny<Func<Climate?, bool>>(),
                It.IsAny<Func<IQueryable<Climate>, IIncludableQueryable<Climate, object>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Climate
            {
                DeviceId = 1,
                Temperature = 2,
                Humidity = 3,
                Pressure = 4,
                Reading = new Reading() { Date = now },
            });
        
        var expected = new ReadingsWebModel
        {
            DeviceId = 1,
            Temperature = 2,
            Humidity = 3,
            Pressure = 4,
            Pm1_0 = 1,
            Pm2_5 = 1,
            Pm10 = 1,
            ReadDate = now
        };
        
        ReadingsWebModel result = await handler.Handle(new GetLatestReadingQuery()
        {
            DeviceId = 1
        }, CancellationToken.None);

        expected.Should().BeEquivalentTo(result);
    } 
    
    [Test]
    public async Task ShouldThrowNoReadingsFoundException()
    {
        qualityRepository.Setup(x => x.GetLastBy(It.IsAny<Func<Quality?, bool>>(),
            It.IsAny<Func<IQueryable<Quality>, IIncludableQueryable<Quality, object>>>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(default(Quality));
        climateRepository.Setup(x => x.GetLastBy(It.IsAny<Func<Climate?, bool>>(),
            It.IsAny<Func<IQueryable<Climate>, IIncludableQueryable<Climate, object>>>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(default(Climate));
        Func<Task> task = async () => await handler.Handle(new GetLatestReadingQuery()
        {
            DeviceId = 1
        }, CancellationToken.None);

        task.Should().ThrowAsync<NotFoundException>().WithMessage("No readings found");
    } 
}