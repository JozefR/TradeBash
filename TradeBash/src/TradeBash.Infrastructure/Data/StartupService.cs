using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TradeBash.Infrastructure.Data
{
    public static class StartupService
    {
        public static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    options.UseSqlServer(configuration.GetConnectionString("SqlServerConnection"));
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    options.UseSqlServer(configuration.GetConnectionString("SqlServerDockerConnection"));
                }
                else
                {
                    // will be created in web project root
                    options.UseSqlite(configuration.GetConnectionString("SqliteConnection"));
                }
            });
            return services;
        }
    }
}