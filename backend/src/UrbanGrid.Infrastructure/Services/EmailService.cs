using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using UrbanGrid.Application.Interfaces;

namespace UrbanGrid.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config) => _config = config;

    public async Task SendWelcomeEmailAsync(
        string toEmail, string name, string tempPassword)
    {
        var smtpHost   = _config["Email:Host"];
        var smtpPort   = int.Parse(_config["Email:Port"]!);
        var smtpUser   = _config["Email:Username"];
        var smtpPass   = _config["Email:Password"];
        var fromEmail  = _config["Email:From"];

        var body = $"""
            <div style="font-family: Arial, sans-serif; max-width: 500px; margin: auto;">
              <h2 style="color: #1565c0;">⚡ Welcome to UrbanGrid</h2>
              <p>Hi <strong>{name}</strong>,</p>
              <p>Your UrbanGrid staff account has been created. Here are your login credentials:</p>

              <div style="background:#f5f5f5; padding:16px; border-radius:8px; margin:16px 0;">
                <p>📧 <strong>Email:</strong> {toEmail}</p>
                <p>🔑 <strong>Temporary Password:</strong>
                  <span style="font-size:18px; color:#1565c0;">
                    <strong>{tempPassword}</strong>
                  </span>
                </p>
              </div>

              <p>
                👉 <a href="https://urbangrid.com/auth/login"
                  style="color:#1565c0;">Click here to Login</a>
              </p>

              <p style="color:#e65100;">
                ⚠️ You will be asked to change your password on first login.
              </p>

              <hr/>
              <p style="color:#999; font-size:12px;">
                This is an automated message from UrbanGrid Admin.
                Do not reply to this email.
              </p>
            </div>
            """;

        using var client = new SmtpClient(smtpHost, smtpPort)
        {
            Credentials = new NetworkCredential(smtpUser, smtpPass),
            EnableSsl = true
        };

        var mail = new MailMessage
        {
            From = new MailAddress(fromEmail!, "UrbanGrid"),
            Subject = "Welcome to UrbanGrid — Your Login Credentials",
            Body = body,
            IsBodyHtml = true
        };
        mail.To.Add(toEmail);

        await client.SendMailAsync(mail);
    }
}
