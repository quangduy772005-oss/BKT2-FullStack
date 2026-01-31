using PCM.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace PCM.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    public async Task SendEmailAsync(string recipient, string subject, string htmlContent)
    {
        try
        {
            // TODO: Implement real email sending using SMTP service
            // For now, just log
            _logger.LogInformation($"Email sent to {recipient}: {subject}");
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to send email to {recipient}");
            throw;
        }
    }

    public async Task SendPasswordResetEmailAsync(string email, string resetLink)
    {
        var subject = "Yêu cầu đặt lại mật khẩu";
        var htmlContent = $@"
            <h2>Yêu cầu đặt lại mật khẩu</h2>
            <p>Bạn đã yêu cầu đặt lại mật khẩu. Vui lòng nhấp vào liên kết dưới đây:</p>
            <a href='{resetLink}'>Đặt lại mật khẩu</a>
            <p>Liên kết này sẽ hết hạn sau 24 giờ.</p>
        ";
        await SendEmailAsync(email, subject, htmlContent);
    }
}
