using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class EmailService
    {
        private readonly string _smtpServer = "smtp.gmail.com"; // SMTP Server (Gmail, Outlook,...)
        private readonly int _smtpPort = 587; // Cổng SMTP
        private readonly string _emailSender = "cakywordvietnam@gmail.com"; // Email của bạn -- cakywordvietnam123.
        private readonly string _emailPassword = "eqjm qmnx twbl gqfe"; // Mật khẩu ứng dụng (SMTP password)

        public async Task<bool> SendEmailAsync(string recipientEmail, string subject, string messageBody)
        {
            try
            {
                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress("Cakyword", _emailSender));
                emailMessage.To.Add(new MailboxAddress("", recipientEmail));
                emailMessage.Subject = subject;

                emailMessage.Body = new TextPart("html")
                {
                    Text = messageBody
                };

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(_smtpServer, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(_emailSender, _emailPassword);
                    await client.SendAsync(emailMessage);
                    await client.DisconnectAsync(true);
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email: {ex.Message}");
                return false;
            }
        }
    }
}
