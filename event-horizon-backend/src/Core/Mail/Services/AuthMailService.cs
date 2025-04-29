using System.Net.Sockets;
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

    public async Task SendVerificationEmailAsync(string email, string userName, string token)
    {
        string template = await LoadTemplateAsync("EmailVerification.html");
        string body = template
            .Replace("{{userName}}", userName)
            .Replace("{{verificationLink}}", $"{_mailSettings.AppUrl}/verify?email={email}&token={token}");

        await SendEmailAsync(email, "Verify your email address", body);
    }

    private async Task<string> LoadTemplateAsync(string templateName)
    {
        string templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates/Emails", templateName);
        return await File.ReadAllTextAsync(templatePath);
    }
    
    private async Task SendEmailAsync(string to, string subject, string htmlBody, int intent = 3)
    {
        if (intent <= 0)
        {
            throw new SocketException(11, "Enable to access to socker server smtp");
        }
        
        MimeMessage message = new MimeMessage();
        message.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.From));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;

        Console.WriteLine("Message Settings");
        Console.WriteLine(_mailSettings.AppUrl);
        Console.WriteLine(_mailSettings.DisplayName);
        Console.WriteLine(_mailSettings.From);
        Console.WriteLine(_mailSettings.Host);
        Console.WriteLine(_mailSettings.Password);
        Console.WriteLine(_mailSettings.Port);
        
        BodyBuilder bodyBuilder = new BodyBuilder
        {
            HtmlBody = htmlBody
        };

        message.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();
        
        await client.ConnectAsync(_mailSettings.Host, _mailSettings.Port, 
            _mailSettings.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto);
            
        if (!string.IsNullOrEmpty(_mailSettings.UserName))
            await client.AuthenticateAsync(_mailSettings.UserName, _mailSettings.Password);

        try
        {
            await client.SendAsync(message);
        }
        catch (SocketException ex) when (ex.ErrorCode == 11)
        {
            Console.WriteLine($"Error en intento {intent}");
            await SendEmailAsync(to, subject, htmlBody, --intent);
        }
        
        Console.WriteLine("Enviado?");
        await client.DisconnectAsync(true);
    }
}