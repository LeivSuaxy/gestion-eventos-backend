namespace event_horizon_backend.Core.Mail;

// MailSettings class to hold configuration settings for sending emails
public class MailSettings
{
    public string Host { get; set; }
    public int Port { get; set; }
    public bool EnableSsl { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string From { get; set; }
    public string DisplayName { get; set; }
    public string AppUrl { get; set; }
}