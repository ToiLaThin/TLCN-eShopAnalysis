
namespace eShopAnalysis.IdentityServer.Utilities
{
    internal interface IEmailSenderService
    {
        Task<bool> SendMail(MailRequest mailContent);
        Task SendEmailAsync(string email, string subject, string htmlMessage);
    }
}
