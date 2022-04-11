using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Persistence;

namespace API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            var host = CreateHostBuilder(args).Build();
            
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                // for activity seeding
                var context = services.GetRequiredService<DataContext>();
                
                // for user seeding
                var userManager = services.GetRequiredService<UserManager<AppUser>>();
                // Apply migration to database or create the database it it not existed
                await context.Database.MigrateAsync();  // use Async version
                // Create seed for database if it is blank
                await Seed.SeedData(context, userManager);  // require Main to be async & return a Task
            }
            catch (Exception ex)
            {

                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occured during migration");
            }

            await host.RunAsync();  // also use Async version
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
