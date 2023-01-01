using Microsoft.EntityFrameworkCore;
using secure_save_pass.Models;

namespace secure_save_pass.Data
{
    public class SecurePassDBContext : DbContext
    {
        public SecurePassDBContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<PasswordInfo> PasswordInfos { get; set; }
    }
}
