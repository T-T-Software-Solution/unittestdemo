using Demo.AppCore.Events;

namespace Demo.AppCore.Interfaces;

public interface INotificationService
{
    Task NotifyStudentCreatedAsync(StudentCreatedEvent eventData);
    Task NotifyStudentUpdatedAsync(StudentUpdatedEvent eventData);
    Task NotifyStudentDeletedAsync(StudentDeletedEvent eventData);
    Task NotifyGradeChangedAsync(GradeChangedEvent eventData);
}

public interface IEmailService
{
    Task SendEmailAsync(string? to, string subject, string body);
}

public interface ISmsService
{
    Task SendSmsAsync(string? phoneNumber, string message);
}