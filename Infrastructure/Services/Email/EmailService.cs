namespace Infrastructure.Services.Email;

using Application.Common.Interfaces;

public class EmailService : IEmailService
{
    public Task SendEmailVerificationAsync(string email, string token, CancellationToken cancellationToken = default)
    {
        // TODO: Implement actual email sending (SMTP, SendGrid, etc.)
        Console.WriteLine($"[EMAIL] Verification sent to: {email}, Token: {token}");
        return Task.CompletedTask;
    }

    public Task SendApplicationStatusUpdateAsync(string email, string applicationId, string status, CancellationToken cancellationToken = default)
    {
        // TODO: Implement actual email sending
        Console.WriteLine($"[EMAIL] Application status update sent to: {email}, Application: {applicationId}, Status: {status}");
        return Task.CompletedTask;
    }

    public Task SendEquipmentAssignmentAsync(string email, string equipmentName, CancellationToken cancellationToken = default)
    {
        // TODO: Implement actual email sending
        Console.WriteLine($"[EMAIL] Equipment assignment sent to: {email}, Equipment: {equipmentName}");
        return Task.CompletedTask;
    }
}
