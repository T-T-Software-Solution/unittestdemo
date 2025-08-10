using Demo.AppCore.Interfaces;

namespace Demo.Notification.Services;

public class EmailService : IEmailService
{
    public async Task SendEmailAsync(string? to, string subject, string body)
    {
        if (string.IsNullOrEmpty(to))
        {
            Console.WriteLine("[EMAIL MOCK] No email address provided");
            return;
        }

        Console.WriteLine($"[EMAIL MOCK] Sending email to: {to}");
        Console.WriteLine($"[EMAIL MOCK] Subject: {subject}");
        Console.WriteLine($"[EMAIL MOCK] Body: {body}");
        Console.WriteLine("[EMAIL MOCK] Email sent successfully");
        
        await Task.Delay(100);
    }
}