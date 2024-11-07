using IdentityUsage.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityUsage.Configuracoes
{
    public static class Services
    {
        public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connection = configuration.GetConnectionString("database");
            services.AddDbContext<DataContext>(options =>
                options
                    .UseMySql(connection, ServerVersion.AutoDetect(connection))
                    .EnableSensitiveDataLogging());

            services.AddIdentity<UserPersonalization, RolePersonalization>()
                .AddEntityFrameworkStores<DataContext>()
                .AddDefaultTokenProviders();
        }
    }
}
