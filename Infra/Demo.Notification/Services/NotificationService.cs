using Demo.AppCore.Events;
using Demo.AppCore.Interfaces;

namespace Demo.Notification.Services;

public class NotificationService : INotificationService
{
    private readonly IEmailService _emailService;
    private readonly ISmsService _smsService;
    private readonly Dictionary<Guid, DateTime> _lastNotificationTime = new();

    public NotificationService(IEmailService emailService, ISmsService smsService)
    {
        _emailService = emailService;
        _smsService = smsService;
    }

    public async Task NotifyStudentCreatedAsync(StudentCreatedEvent eventData)
    {
        if (ShouldDebounce(eventData.StudentId)) return;

        var subject = "Welcome to Demo University!";
        var emailBody = $"Dear {eventData.FirstName} {eventData.LastName},\n\nYour student account has been created successfully.\nStudent No: {eventData.StudentNo}\n\nBest regards,\nDemo University";
        var smsMessage = $"Welcome {eventData.FirstName}! Your student account (#{eventData.StudentNo}) is ready.";

        await _emailService.SendEmailAsync(eventData.Email, subject, emailBody);
        await _smsService.SendSmsAsync(eventData.Phone, smsMessage);
        
        UpdateLastNotificationTime(eventData.StudentId);
    }

    public async Task NotifyStudentUpdatedAsync(StudentUpdatedEvent eventData)
    {
        if (ShouldDebounce(eventData.StudentId)) return;

        var subject = "Student Profile Updated";
        var emailBody = $"Dear {eventData.FirstName} {eventData.LastName},\n\nYour student profile has been updated.\nStudent No: {eventData.StudentNo}\n\nBest regards,\nDemo University";
        var smsMessage = $"Hello {eventData.FirstName}! Your profile has been updated.";

        await _emailService.SendEmailAsync(eventData.Email, subject, emailBody);
        await _smsService.SendSmsAsync(eventData.Phone, smsMessage);
        
        UpdateLastNotificationTime(eventData.StudentId);
    }

    public async Task NotifyStudentDeletedAsync(StudentDeletedEvent eventData)
    {
        Console.WriteLine($"[NOTIFICATION] Student {eventData.StudentNo} ({eventData.FirstName} {eventData.LastName}) account deactivated");
        await Task.CompletedTask;
    }

    public async Task NotifyGradeChangedAsync(GradeChangedEvent eventData)
    {
        if (ShouldDebounce(eventData.StudentId)) return;

        Console.WriteLine($"[NOTIFICATION] Grade changed for student {eventData.StudentNo}: {eventData.OldGrade:F2}% → {eventData.NewGrade:F2}%");
        
        await Task.Delay(50);
        UpdateLastNotificationTime(eventData.StudentId);
    }

    private bool ShouldDebounce(Guid studentId)
    {
        if (!_lastNotificationTime.TryGetValue(studentId, out var lastTime))
            return false;

        return DateTime.UtcNow.Subtract(lastTime).TotalSeconds < 2;
    }

    private void UpdateLastNotificationTime(Guid studentId)
    {
        _lastNotificationTime[studentId] = DateTime.UtcNow;
    }
}