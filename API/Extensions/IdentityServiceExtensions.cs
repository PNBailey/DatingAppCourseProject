using System.Text;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace API.Extensions
{

    // We use an extension class and method here to cleanup our startup class
    public static class IdentityServiceExtensions
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config) 
        {

            services.AddIdentityCore<AppUser>(opt => { // This is configuring our .NET Identity settings. If we weere creating an MVC application where the client was served by .NET through something like Razor pages, we would use AddIdentity rather than AddIdentityCore. Using the options, we can change the default settings of Identity 
                opt.Password.RequireNonAlphanumeric = false; // This changes the default settings for Password settings within Identity Core
            })
            // What we do now is add the roles we need in our app
            .AddRoles<AppRole>() // This is the role type we configured
            .AddRoleManager<RoleManager<AppRole>>()
            .AddSignInManager<SignInManager<AppUser>>()
            .AddRoleValidator<RoleValidator<AppRole>>()
            .AddEntityFrameworkStores<DataContext>(); // This sets up our database with all of the tables we need to create the .NET identity tables

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme) // We have to add the authentication middleware (Microsoft.AspNetCore.Authentication.JwtBearer) to our Configure services method here.  
             .AddJwtBearer (options => 
             {
                 options.TokenValidationParameters = new TokenValidationParameters 
                 {
                     ValidateIssuerSigningKey = true, // Our server signs the token, and we need to tell it to actually validate this token is correct
                     
                     // Then we need to issue a valid sign in key 
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"])), 
                    ValidateIssuer = false, // This is the API server. We set this to false
                    ValidateAudience = false // This is our angular app. We set this to false


                 };
             });

             return services;
        }
    }
}