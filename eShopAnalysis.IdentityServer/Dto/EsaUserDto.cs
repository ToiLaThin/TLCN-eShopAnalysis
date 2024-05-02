using eShopAnalysis.IdentityServer.Models;
using System.Text.Json.Serialization;

namespace eShopAnalysis.IdentityServer.Dto
{
    public record EsaUserDto
    {
        public string Email { get; init; }

        public string Username { get; init; }

        public string AvatarUrl { get; init; }

        public EsaUserDto() { }

        public EsaUserDto(string email, string username, string avatarUrl)
        {
            Email = email;
            Username = username;
            AvatarUrl = avatarUrl;
        }

        public EsaUserDto(EsaUser esaUser)
        {
            Email = esaUser.Email;
            Username = esaUser.UserName;
            AvatarUrl = esaUser.AvatarUrl;
        }
    }
}
