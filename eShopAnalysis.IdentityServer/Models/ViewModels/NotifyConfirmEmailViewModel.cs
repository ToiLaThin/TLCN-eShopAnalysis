namespace eShopAnalysis.IdentityServer.Models.ViewModels
{
    public class NotifyConfirmEmailViewModel
    {
        public string ReturnUrl { get; set; }

        public string Email { get; set; }

        public string AccountActivationToken { get; set; }

    }
}
