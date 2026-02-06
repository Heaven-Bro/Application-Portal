namespace Application.Common.Interfaces;

public interface IEmailService
{
    Task SendEmailVerificationAsync(string email, string token, CancellationToken cancellationToken = default);
    Task SendApplicationStatusUpdateAsync(string email, string applicationId, string status, CancellationToken cancellationToken = default);
    Task SendEquipmentAssignmentAsync(string email, string equipmentName, CancellationToken cancellationToken = default);
}







