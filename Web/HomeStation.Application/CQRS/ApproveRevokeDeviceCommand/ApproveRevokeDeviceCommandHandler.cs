using HomeStation.Application.Common.Enums;
using HomeStation.Application.Common.Exceptions;
using HomeStation.Application.Common.Interfaces;
using HomeStation.Domain.Common.Entities;
using HomeStation.Domain.Common.Interfaces;

namespace HomeStation.Application.CQRS.ApproveRevokeDeviceCommand;

/// <summary>
/// The class handling approving and revoking device command
/// </summary>
public class ApproveRevokeDeviceCommandHandler : ICommandHandler<ApproveRevokeDeviceCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    /// <inheritdoc cref="ICommandHandler{ApproveRevokeDeviceCommand}"/>
    public ApproveRevokeDeviceCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    /// <summary>
    /// Handles approving or revoking device
    /// </summary>
    /// <param name="command">The <see cref="ApproveRevokeDeviceCommand"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <param name="identity">The optional device id.</param>
    /// <exception cref="Exception">If device doesn't exist.</exception>
    public async Task Handle(ApproveRevokeDeviceCommand command, CancellationToken cancellationToken, string? identity = null)
    {
        using (_unitOfWork)
        {
            Device? device = await _unitOfWork.DeviceRepository.GetObjectBy(x => x?.Id == command.Id && x.Name == command.Name, cancellationToken: cancellationToken);

            if (device == null)
            {
                throw new ApproveException("No device to approve.");
            }

            device.IsKnown = command.Operation == OperationType.Approve;
            
            _unitOfWork.DeviceRepository.UpdateAsync(device);
            await _unitOfWork.Save(cancellationToken);
        }
    }
}