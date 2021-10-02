using System.Text;
using System.Threading.Tasks;
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

                 // We add the below options so that the client can send up the token as a query 
                 // string. We need this for Signal R as Signalr does not send authentication headers   // like controllers do. Once the below has been completed, we need to add XXXX to our  // CORS config in our startup class. Adding this method means that for Signal R we can // always use a query string, whereas the api controllers will just use the            // authentication header which is defined above. Below, we add a new JWTBearerEvents
                 // object to the JWTBearerOptions.Events object. This allows us to assign a delegate 
                 // (query string token) to the events that we want. The event we want to add this to 
                 // is the OnMessageReceived event. This OnMessageReceived event is invoked when a 
                 // when a protocol (WebSocket in SignalR's case) message is first received. 

                options.Events = new JwtBearerEvents 
                {
                    OnMessageReceived = context => 
                    {

                        // By default, Signalr will send up the token as a query string with the key,   // "access_token", this is why we use .Query["access_token"] to access 
                        // the query string. 

                        var accessToken = context.Request.Query["access_token"];

                        // We then need to check the path of the request. We only want to do this 
                        // for signal r requests so we need to check the url request path to 
                        // see if it matches (or partialy matches) the end point path that we specified // in our startup class.

                        var path = context.HttpContext.Request.Path;

                        if(!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    }
                  
                };
             
             });


             services.AddAuthorization(opt => {
                 opt.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
                 opt.AddPolicy("ModeratePhotoRole", policy => policy.RequireRole("Admin", "Moderator"));
             }); // This adds the policies that we can then use in our controllers Authorize attributes to restrict access to certain end points

             return services;
        }
    }
}