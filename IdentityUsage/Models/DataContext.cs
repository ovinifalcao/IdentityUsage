using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityUsage.Models
{
    public class DataContext : IdentityDbContext<UserPersonalization, RolePersonalization, int>
    {
        public DataContext(DbContextOptions<DataContext> opt) : base(opt) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<RolePersonalization>().HasData(new List<RolePersonalization>()
            {
                new RolePersonalization {Id = 1, Name = AppRoles.Administrator, NormalizedName = AppRoles.Administrator},
                new RolePersonalization {Id = 2, Name = AppRoles.Consumer, NormalizedName = AppRoles.Consumer},
            });
        }
    }
}
