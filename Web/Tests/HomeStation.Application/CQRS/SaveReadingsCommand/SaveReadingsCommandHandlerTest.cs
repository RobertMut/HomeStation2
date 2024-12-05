using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using HomeStation.Application.Common.Exceptions;
using HomeStation.Application.Common.Interfaces;
using HomeStation.Application.Common.Virtual;
using HomeStation.Application.CQRS.SaveReadingsCommand;
using HomeStation.Application.Tests.TestsHelpers;
using HomeStation.Domain.Common.Entities;
using Microsoft.EntityFrameworkCore.Query;
using Moq;

namespace HomeStation.Application.Tests.CQRS;

[TestFixture]
[TestOf(typeof(SaveReadingsCommandHandler))]
[ExcludeFromCodeCoverage]
public class SaveReadingsCommandHandlerTest
{
    private Mock<Repository<Quality>> qualityRepository;
    private Mock<Repository<Climate>> climateRepository;
    private Mock<Repository<Device>> deviceRepository;

    private IUnitOfWork unitOfWork;
    private SaveReadingsCommandHandler handler;
    
    [SetUp]
    public void SetUp()
    {
        qualityRepository = new Mock<Repository<Quality>>(Mock.Of<IAirDbContext>());
        climateRepository = new Mock<Repository<Climate>>(Mock.Of<IAirDbContext>());
        deviceRepository = new Mock<Repository<Device>>(Mock.Of<IAirDbContext>());
        unitOfWork = new UnitOfWorkMock(qualityRepository, climateRepository, deviceRepository).Get;
        
        handler = new SaveReadingsCommandHandler(unitOfWork);
    }

    [Test]
    public async Task HandlerShouldReturnExpectedResult()
    {
        deviceRepository.Setup(x => x.GetObjectBy(It.IsAny<Func<Device?, bool>>(),
                It.IsAny<Func<IQueryable<Device>, IIncludableQueryable<Device, object>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Device()
            {
                Id = 1,
                Name = "TEST",
                IsKnown = true
            });

        climateRepository.Setup(x => x.InsertAsync(It.IsAny<Climate?>(), It.IsAny<CancellationToken>())).Verifiable();
        qualityRepository.Setup(x => x.InsertAsync(It.IsAny<Quality?>(), It.IsAny<CancellationToken>())).Verifiable();
        
        Func<Task> f = () => handler.Handle(new SaveReadingsCommand
        {
            Temperature = 1,
            Humidity = 2,
            Pressure = 3,
            Pm1_0 = 4,
            Pm2_5 = 5,
            Pm10 = 6
        }, CancellationToken.None, "TEST");

        f.Should().NotThrowAsync();
        
        climateRepository.Verify(x => x.InsertAsync(It.Is<Climate>(c => 
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            c.Temperature == 1.0d && 
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            c.Humidity == 2.0d &&
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            c.Pressure == 3.0d), It.IsAny<CancellationToken>()), Times.Once);
        
        qualityRepository.Verify(x => x.InsertAsync(It.Is<Quality>(q => 
            q.Pm1_0 == 4 && 
            q.Pm2_5 == 5 &&
            q.Pm10 == 6), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Test]
    public async Task HandlerShouldThrowDeviceNotFoundException()
    {
        deviceRepository.Setup(x => x.GetObjectBy(It.IsAny<Func<Device?, bool>>(),
                It.IsAny<Func<IQueryable<Device>, IIncludableQueryable<Device, object>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Device()
            {
                Id = 1,
                Name = "TEST",
                IsKnown = false
            });

        climateRepository.Setup(x => x.InsertAsync(It.IsAny<Climate?>(), It.IsAny<CancellationToken>())).Verifiable();
        qualityRepository.Setup(x => x.InsertAsync(It.IsAny<Quality?>(), It.IsAny<CancellationToken>())).Verifiable();
        
        Func<Task> f = () => handler.Handle(new SaveReadingsCommand
        {
            Temperature = 1,
            Humidity = 2,
            Pressure = 3,
            Pm1_0 = 4,
            Pm2_5 = 5,
            Pm10 = 6
        }, CancellationToken.None, "TEST");

        f.Should().ThrowAsync<NotFoundException>().WithMessage("Device is not known");
        
        climateRepository.Verify(x => x.InsertAsync(It.IsAny<Climate?>(), It.IsAny<CancellationToken>()), Times.Never);
        qualityRepository.Verify(x => x.InsertAsync(It.IsAny<Quality?>(), It.IsAny<CancellationToken>()), Times.Never);
    }
    
    [Test]
    public async Task HandlerShouldThrowInvalidReadings()
    {
        deviceRepository.Setup(x => x.GetObjectBy(It.IsAny<Func<Device?, bool>>(),
                It.IsAny<Func<IQueryable<Device>, IIncludableQueryable<Device, object>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Device()
            {
                Id = 1,
                Name = "TEST",
                IsKnown = true
            });

        climateRepository.Setup(x => x.InsertAsync(It.IsAny<Climate?>(), It.IsAny<CancellationToken>())).Verifiable();
        qualityRepository.Setup(x => x.InsertAsync(It.IsAny<Quality?>(), It.IsAny<CancellationToken>())).Verifiable();
        
        Func<Task> f = () => handler.Handle(new SaveReadingsCommand
        {
            Temperature = 1,
            Humidity = 2,
            Pressure = 0,
            Pm1_0 = 4,
            Pm2_5 = 5,
            Pm10 = 6
        }, CancellationToken.None, "TEST");

        f.Should().ThrowAsync<ReadingsException>().WithMessage("Invalid readings. Pressure cannot be 0");
        
        climateRepository.Verify(x => x.InsertAsync(It.IsAny<Climate?>(), It.IsAny<CancellationToken>()), Times.Never);
        qualityRepository.Verify(x => x.InsertAsync(It.IsAny<Quality?>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}