﻿using TradeBash.SharedKernel.Interfaces;
using TradeBash.Infrastructure.Data;
using TradeBash.UnitTests;
using TradeBash.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.AspNetCore.TestHost;
using System.Linq;

namespace TradeBash.FunctionalTests
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder
                .UseSolutionRelativeContentRoot("src/TradeBash.Web")
                .ConfigureServices(services =>
            {
                // Remove the app's ApplicationDbContext registration.
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                        typeof(DbContextOptions<AppDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add ApplicationDbContext using an in-memory database for testing.
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                });

                //// Create a new service provider.
                //var serviceProvider = new ServiceCollection()
                //    .AddEntityFrameworkInMemoryDatabase()
                //    .BuildServiceProvider();

                //// Add a database context (AppDbContext) using an in-memory
                //// database for testing.
                //services.AddDbContext<AppDbContext>(options =>
                //{
                //    options.UseInMemoryDatabase("InMemoryDbForTesting");
                //    options.UseInternalServiceProvider(serviceProvider);
                //});

                // Build the service provider.
                var sp = services.BuildServiceProvider();

                // Create a scope to obtain a reference to the database
                // context (AppDbContext).
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<AppDbContext>();

                    var logger = scopedServices
                        .GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();

                    // Ensure the database is created.
                    db.Database.EnsureCreated();

                    try
                    {
                        //SeedData.PopulateTestData(db);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "An error occurred seeding the " +
                            $"database with test messages. Error: {ex.Message}");
                    }
                }
            });
        }
    }
}
