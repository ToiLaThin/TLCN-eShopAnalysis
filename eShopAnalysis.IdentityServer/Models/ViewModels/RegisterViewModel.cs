namespace eShopAnalysis.IdentityServer.Models.ViewModels
{
    public class RegisterViewModel
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public string PasswordConfirmed { get; set; }

        public string ReturnUrl { get; set; }

        public string Email { get; set; }
    }
}
