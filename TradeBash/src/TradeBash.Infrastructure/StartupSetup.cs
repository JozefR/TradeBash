using System.Runtime.InteropServices;
using TradeBash.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TradeBash.Infrastructure
{
    public static class StartupSetup
    {
        public static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    options.UseSqlServer(configuration.GetConnectionString("SqlServerConnection"), 
                        b => b.MigrationsAssembly("TradeBash.Web"));
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    options.UseSqlServer(configuration.GetConnectionString("SqlServerDockerConnection"),
                        b => b.MigrationsAssembly("TradeBash.Web"));
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
