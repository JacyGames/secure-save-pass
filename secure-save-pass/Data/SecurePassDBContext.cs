using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using secure_save_pass.Models;

namespace secure_save_pass.Data
{
    public class SecurePassDBContext: IdentityDbContext<IdentityUser>
    {
        public SecurePassDBContext(DbContextOptions options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
        public DbSet<PasswordInfo> PasswordInfos { get; set; }

        public DbSet<UserInfo> UserInfos { get; set; }
    }
}
