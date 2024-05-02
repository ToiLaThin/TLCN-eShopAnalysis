using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace eShopAnalysis.IdentityServer.Models
{
    public class EsaIdentityDbContext: IdentityDbContext<EsaUser, IdentityRole, string>
    {
        public EsaIdentityDbContext(DbContextOptions<EsaIdentityDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<EsaUser>(u =>
            {
                u.Property(u => u.AvatarUrl).IsRequired(false).HasMaxLength(500);
            });
        }
    }
}
