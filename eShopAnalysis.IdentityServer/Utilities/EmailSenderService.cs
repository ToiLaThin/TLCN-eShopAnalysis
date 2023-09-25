using MailKit.Security;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Options;

namespace eShopAnalysis.IdentityServer.Utilities
{
    public class EmailSenderService : IEmailSenderService
    {
        private readonly MailOptions _options;

        public EmailSenderService(IOptions<MailOptions> options)
        {
            _options = options.Value;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> SendMail(MailRequest mailContent)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_options.Mail);
            email.To.Add(MailboxAddress.Parse(mailContent.ToEmail));
            email.Subject = mailContent.Subject;
            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = mailContent.Body;
            email.Body = bodyBuilder.ToMessageBody();


            var smtp = new SmtpClient();
            smtp.Connect(_options.Host, _options.Port, SecureSocketOptions.StartTls);
            //please block avast anti virus
            //ko can bat imap cua nguoi nhan
            smtp.Authenticate(_options.Mail, _options.Password);
            try
            {
                await smtp.SendAsync(email);
                smtp.Disconnect(true);
                return true;
            }
            catch (Exception ex) { }
            return false;
        }
    }
}
