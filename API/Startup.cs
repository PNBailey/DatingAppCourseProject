using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.Extensions;
using API.Interfaces;
using API.Middleware;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace API
{
    public class Startup
    {
        private readonly IConfiguration _config;
        public Startup(IConfiguration config) // This is the convention for creating a constructor for the startup file 
        {
            _config = config;
            
        }

        public IConfiguration Configuration { get; }

        
        public void ConfigureServices(IServiceCollection services) // This is often referred to as our dependancy injection container. If we want to make a class or a service available to other areas of our app, we can add them inside this container and .net core is going to take care of the creation of these classes and the destruction of the classes. This method gets called by the runtime. Use this method to add services to the container.
        {

            services.AddApplicationServices(_config); // We call the extension method we created in our Extensions class
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
            });
             services.AddCors(); // We add this to allow “Cross origin resource sharing” to prevent the CORS error being triggered when using a http request between our client and our api. 

              services.AddIdentityServices(_config); 
             }

            
        

        // This method below gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<ExceptionMiddleware>(); // We add our customer exceptionmiddleware exception handling code to the top of the configure method
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(policy => policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200")); // We add this to allow “Cross origin resource sharing” to prevent the CORS error being triggered when using a http request between our client and our api. WE MUST ADD THIS BETWEEN UseRouting AND UseAuthorization. The parameter we pass in here is the policy we are going to use. We have to AllowAnyHeader, AllowAnyMethod (put requests, get requests etc) and specify the origins we want to allow using 'WithOrigins' which is: http://localhost:4200. What this says is that we will allow any header to be sent and any method to be used (get request put request etc) as long as it comes from the origin http://localhost:4200

            app.UseAuthentication(); // We add this when we want to use Authentication. This must go between the UseCors and the UseAuthorization 

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers(); // 
            });
        }
    }
}
