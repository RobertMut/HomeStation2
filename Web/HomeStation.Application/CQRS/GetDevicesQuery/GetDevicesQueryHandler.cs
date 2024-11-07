using System.Linq.Expressions;
using HomeStation.Application.Common.Interfaces;
using HomeStation.Domain.Common.Entities;
using HomeStation.Domain.Common.Interfaces;

namespace HomeStation.Application.CQRS.GetDevicesQuery;

/// <summary>
/// The class handling getting devices query
/// </summary>
public class GetDevicesQueryHandler : IQueryHandler<GetDevicesQuery, IEnumerable<DeviceWebModel>>
{
    private readonly IUnitOfWork _unitOfWork;

    /// <inheritdoc cref="IQueryHandler{GetDevicesQuery,IEnumerable}"/>
    public GetDevicesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    /// <summary>
    /// Handles getting devices
    /// </summary>
    /// <param name="query">The <see cref="GetDevicesQuery"/></param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/></param>
    /// <returns>The <see cref="IEnumerable{DeviceWebModel}"/> of <see cref="DeviceWebModel"/></returns>
    public async Task<IEnumerable<DeviceWebModel>> Handle(GetDevicesQuery query, CancellationToken cancellationToken)
    {
        List<Device> devices = new List<Device>();
        
        using (IUnitOfWork unitOfWork = _unitOfWork)
        {
            devices.AddRange(unitOfWork.DeviceRepository.GetAll());
        }
        
        return devices.Select(x => new DeviceWebModel()
        {
            Id = x.Id,
            Name = x.Name,
            IsKnown = x.IsKnown
        });
    }
}