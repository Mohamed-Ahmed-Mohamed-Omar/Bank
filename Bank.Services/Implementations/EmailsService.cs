using Bank.Data.Helpers;
using Bank.Services.Abstracts;
using MailKit.Net.Smtp;
using MimeKit;

namespace Bank.Services.Implementations
{
    internal class EmailsService : IEmailsService
    {
        private readonly MailSetting _setting;

        public EmailsService(MailSetting setting)
        {
            _setting = setting;
        }

        public async Task<string> SendEmail(string mailTo, string Message, string? reason)
        {
            using (var smtpClient = new SmtpClient())
            {
                await smtpClient.ConnectAsync(_setting.Host, _setting.Port, true);
                await smtpClient.AuthenticateAsync(_setting.Email, _setting.Password);
                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = $"{Message}",
                    TextBody = "Wellcome"
                };
                var mailMessage = new MimeMessage()
                {
                    Body = bodyBuilder.ToMessageBody()
                };
                mailMessage.From.Add(new MailboxAddress(_setting.DisplayName, _setting.Email));
                mailMessage.To.Add(new MailboxAddress("Test", mailTo));
                mailMessage.Subject = reason == null ? "Welcome" : reason;
                await smtpClient.SendAsync(mailMessage);
                smtpClient.Disconnect(true);
            }
            return "Success";
        }
    }
}
