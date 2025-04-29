using event_horizon_backend.Core.Utils;

namespace event_horizon_backend.Core.Mail.Services;

using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

public class AuthMailService
{
    private readonly MailSettings _mailSettings;

    public AuthMailService(IOptions<MailSettings> mailSettings)
    {
        _mailSettings = mailSettings.Value;
    }

    public async Task SendVerificationEmailAsync(string email, string userName)
    {
        string template = await LoadTemplateAsync("EmailVerification.html");
        string token = CodeGeneration.New();
        string body = template
            .Replace("{{userName}}", userName)
            .Replace("{{verificationLink}}", $"{_mailSettings.AppUrl}/verify?token={token}");

        await SendEmailAsync(email, "Verify your email address", body);
    }

    private async Task<string> LoadTemplateAsync(string templateName)
    {
        string templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates/Emails", templateName);
        return await File.ReadAllTextAsync(templatePath);
    }
    
    private async Task SendEmailAsync(string to, string subject, string htmlBody)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.From));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = htmlBody
        };

        message.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();
        
        await client.ConnectAsync(_mailSettings.Host, _mailSettings.Port, 
            _mailSettings.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto);
            
        if (!string.IsNullOrEmpty(_mailSettings.UserName))
            await client.AuthenticateAsync(_mailSettings.UserName, _mailSettings.Password);
            
        await client.SendAsync(message);
        Console.WriteLine("Enviado?");
        await client.DisconnectAsync(true);
    }
}