using API.Data;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace API.Extensions
{

    // We use an extension class and method here to cleanup our startup class
    public static class ApplicationServiceExtensions // An extension class must contain the static keyword 
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        { // The method must also be static. We are extending the IServiceCollection and creating this method to creae our application service

            services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings")); // The "cloudinarysettings" is the name we gave it in our appsettings.json file. We pass in our CloudinarySettings type and this uses our strongly typed configuration settings class we created

            services.AddIdentityCore<AppUser>(opt => { // Here we configure Identity. If we wre building an MVC type application where our client side of the aplication was being served by .NET and we were using Razor pages (which are served by the .NET server) then we could use the default Identity. 
                opt.Password.RequireNonAlphanumeric = false; // By default, Identity requires complex passwords. If we wanted to configure this, we can do that here, like changing the RequireNonAlphanumeric property for example 
                
            })

                .AddRoles<AppRole>() // Here we chain on some services
                .AddRoleManager<RoleManager<AppRole>>() // We add a Role Manager 
                .AddSignInManager<SignInManager<AppUser>>()
                .AddRoleValidator<RoleValidator<AppRole>>()
                .AddEntityFrameworkStores<DataContext>(); // This sets up our database with all of the tables we need to create the .NET identity tables

            services.AddScoped<ITokenService, TokenService>(); // This is what we need to add so our token service to enable us to use the service in other parts of our app. The Addscoped is scoped to the lifetime of the http request in this case. When the request comes in and we have this service injected into that particular controller then a new instance of this service is created and when the request is finished, the service is disposed. We use this one almost all of the time. 

            services.AddScoped<IPhotoService, PhotoService>();

            services.AddScoped<LogUserActivity>();

            services.AddScoped<ILikesRepository, LikesRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly); // We must also tell the AutoMapper where the profiles are located in our project. This is enough for automapper to go ahead and find those profiles (CreateMaps) we created there. 


            services.AddDbContext<DataContext>(options => // As we want to inject the DataContext into other parts of our app, we use our configure services method. We also need to create a connection string for our database. We use the AddDbContext to add this class. We pass in some options.
            {
                options.UseSqlite(config.GetConnectionString("DefaultConnection")); // We pass this a connection string. To add a connection string, we add it to our configuration files. We add it to the devlopment file as we don't mind people seeing this. We add it like this "ConnectionStrings" : { "DefaultConnection": "Data source=datingapp.db". This is simply the name of the file where we want to store our database as we are using Sqlite. As we inject our config file into this startup class, we get access to the Connection string that we created in the config file 

            });

            return services;

        }
    }
}