using API.Data;
using API.Helpers;
using API.Interfaces;
using API.Services;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace API.Extensions
{

    // We use an extension class and method here to cleanup our startup class
    public static class ApplicationServiceExtensions // An extension class must contain the static keyword 
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config) { // The method must also be static. We are extending the IServiceCollection and creating this method to creae our application service

        
            services.AddScoped<ITokenService, TokenService>(); // This is what we need to add so our token service to enable us to use the service in other parts of our app. The Addscoped is scoped to the lifetime of the http request in this case. When the request comes in and we have this service injected into that particular controller then a new instance of this service is created and when the request is finished, the service is disposed. We use this one almost all of the time. 

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