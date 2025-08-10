using Demo.AppCore.Interfaces;

namespace Demo.Notification.Services;

public class SmsService : ISmsService
{
    public async Task SendSmsAsync(string? phoneNumber, string message)
    {
        if (string.IsNullOrEmpty(phoneNumber))
        {
            Console.WriteLine("[SMS MOCK] No phone number provided");
            return;
        }

        Console.WriteLine($"[SMS MOCK] Sending SMS to: {phoneNumber}");
        Console.WriteLine($"[SMS MOCK] Message: {message}");
        Console.WriteLine("[SMS MOCK] SMS sent successfully");
        
        await Task.Delay(100);
    }
}