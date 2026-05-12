namespace UrbanGrid.Application.Interfaces;

public interface IEmailService
{
    Task SendWelcomeEmailAsync(string toEmail, string name, string tempPassword);
}
