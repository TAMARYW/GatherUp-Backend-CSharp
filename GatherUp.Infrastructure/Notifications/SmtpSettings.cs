namespace GatherUp.Infrastructure.Notifications;
public class SmtpSettings
{
    public required string Host { get; set; }
    public int Port { get; set; } = 587;
    public required string SenderEmail { get; set; }
    public string SenderName { get; set; } = "GatherUp";
    public required string AppPassword { get; set; }
    public bool EnableSsl { get; set; } = true;
}