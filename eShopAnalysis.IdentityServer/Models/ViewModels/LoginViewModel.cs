using Microsoft.AspNetCore.Authentication;

namespace eShopAnalysis.IdentityServer.Models.ViewModels
{
    public class LoginViewModel
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public string ReturnUrl { get; set; }

        public IEnumerable<AuthenticationScheme> ExternalProviders { get; set; }
    }
}