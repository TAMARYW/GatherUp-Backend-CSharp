using System;
using System.Net;
using System.Net.Mail;
using GatherUp.Core.Interfaces;

namespace GatherUp.Infrastructure.Notifications;

public class SmtpEmailService : IEmailService
{
    private readonly SmtpSettings _settings;

    public SmtpEmailService(SmtpSettings settings)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }

    public void SendEmail(string toAddress, string subject, string body)
    {
        using var client = new SmtpClient(_settings.Host, _settings.Port)
        {
            Credentials = new NetworkCredential(_settings.SenderEmail, _settings.AppPassword),
            EnableSsl = _settings.EnableSsl
        };

        using var message = new MailMessage
        {
            From = new MailAddress(_settings.SenderEmail, _settings.SenderName),
            Subject = subject,
            Body = body,
            IsBodyHtml = false
        };
        message.To.Add(toAddress);

        client.Send(message);
    }
}