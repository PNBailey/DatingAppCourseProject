using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace API
{
    public class Program
    {
        public static async Task Main(string[] args) //Every .net app has a program.cs file. Within this class is a main method. When the .net run command executes, it’s looking for this method and it’s going to run any of the code inside the main method. Inside the main method on an api app the CreateHostBuilder method is called. This method runs the startup.cs file…
        {
            var host = CreateHostBuilder(args).Build();

            using var scope = host.Services.CreateScope(); // Here we create a scope for the services that we are to create in this part

            var services = scope.ServiceProvider;

            try {
                var context = services.GetRequiredService<DataContext>();
                await context.Database.MigrateAsync(); // Using this means that we don't have to use 'dotnet ef Database update'. Once this has been entered, all we need to do is restart out app to apply any changes to the database
                await Seed.SeedUsers(context);
            } catch (Exception ex) {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occured during migration");
            }

            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
