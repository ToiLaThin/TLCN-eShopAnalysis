using Microsoft.AspNetCore.Identity;

namespace eShopAnalysis.IdentityServer.Models
{
    public class EsaUser: IdentityUser<string>
    {
        public string AvatarUrl { get; set; }

        //so the Id, ConcurrencyStamp get generated, but why the base constructor not called, and it not give Id, ConcurrencyStamp any value
        //check why this get callled so many times
        public EsaUser(): base() {
            //this.Id = base.Id;
            //this.ConcurrencyStamp = base.ConcurrencyStamp;
            // not have any value

            Id = Guid.NewGuid().ToString();
            SecurityStamp = Guid.NewGuid().ToString();

        }
    }
}
