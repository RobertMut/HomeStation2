using System.Diagnostics.CodeAnalysis;
using HomeStation.Application.Common.Interfaces;
using HomeStation.Application.Common.Virtual;
using HomeStation.Domain.Common.Entities;
using Moq;

namespace HomeStation.Application.Tests.TestsHelpers;

[ExcludeFromCodeCoverage]
public class UnitOfWorkMock
{
    public UnitOfWorkMock(Mock<Repository<Quality>> qualityRepositoryMock = default,
        Mock<Repository<Climate>> climateRepositoryMock = default,
        Mock<Repository<Device>> deviceRepositoryMock = default)
    {
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        
        if (qualityRepositoryMock != null)
        {
            unitOfWorkMock.Setup(x => x.QualityRepository)
                .Returns(qualityRepositoryMock.Object);
        }
        
        if (climateRepositoryMock != null)
        {
            unitOfWorkMock.Setup(x => x.ClimateRepository)
                .Returns(climateRepositoryMock.Object);
        }
        
        if (deviceRepositoryMock != null)
        {
            unitOfWorkMock.Setup(x => x.DeviceRepository)
                .Returns(deviceRepositoryMock.Object);
        }

        unitOfWorkMock.Setup(x => x.Save(It.IsAny<CancellationToken>()));
        Get = unitOfWorkMock.Object;
    }

    public IUnitOfWork Get { get; private set; }
}