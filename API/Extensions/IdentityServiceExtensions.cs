using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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