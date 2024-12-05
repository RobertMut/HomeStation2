using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using HomeStation.Application.Common.Interfaces;
using HomeStation.Infrastructure.Persistence;
using HomeStation.Infrastructure.Repositories;
using Moq;

namespace HomeStation.Infrastructure.Tests.Repositories;

[TestFixture]
[TestOf(typeof(UnitOfWork))]
[ExcludeFromCodeCoverage]
public class UnitOfWorkTest
{
    private UnitOfWork _unitOfWork;

    [Test]
    public void UnitOfWorkCreatesRepos()
    {
        var context = Mock.Of<IAirDbContext>();
        _unitOfWork = new UnitOfWork(context);

        var climateRepository = _unitOfWork.ClimateRepository;
        var qualityRepository = _unitOfWork.QualityRepository;
        var deviceRepository = _unitOfWork.DeviceRepository;

        climateRepository.Should().NotBeNull();
        qualityRepository.Should().NotBeNull();
        deviceRepository.Should().NotBeNull();
    }
    
    [Test]
    public async Task UnitOfWorkShouldInvokeSave()
    {
        var context = new Mock<AirDbContext>();
        context.Setup(x => x.SaveChangesAsync( It.IsAny<CancellationToken>()))
            .Verifiable();
        
        _unitOfWork = new UnitOfWork(context.Object);

        await _unitOfWork.Save(CancellationToken.None);

        context.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Test]
    public async Task UnitOfWorkShouldDisposeAsync()
    {
        var context = new Mock<AirDbContext>();
        context.Setup(x => x.DisposeAsync())
            .Verifiable();
        
        _unitOfWork = new UnitOfWork(context.Object);

        _unitOfWork.Dispose(true);

        context.Verify(x => x.DisposeAsync(), Times.Once);
    }
    
    [Test]
    public async Task UnitOfWorkShouldDispose()
    {
        var context = new Mock<AirDbContext>();
        context.Setup(x => x.DisposeAsync())
            .Verifiable();
        
        _unitOfWork = new UnitOfWork(context.Object);

        _unitOfWork.Dispose();

        context.Verify(x => x.DisposeAsync(), Times.Once);
    }
}