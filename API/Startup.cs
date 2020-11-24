using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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

            services.AddDbContext<DataContext>(options => // As we want to inject the DataContext into other parts of our app, we use our configure services method. We also need to create a connection string for our database. We use the AddDbContext to add this class. We pass in some options.
            {  
                options.UseSqlite(_config.GetConnectionString("DefaultConnection")); // We pass this a connection string. To add a connection string, we add it to our configuration files. We add it to the devlopment file as we don't mind people seeing this. We add it like this "ConnectionStrings" : { "DefaultConnection": "Data source=datingapp.db". This is simply the name of the file where we want to store our database as we are using Sqlite. As we inject our config file into this startup class, we get access to the Connection string that we created in the config file 

            });
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
            });
             services.AddCors(); // We add this to allow “Cross origin resource sharing” to prevent the CORS error being triggered when using a http request between our client and our api. 
        }

        // This method below gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) // This checks to see whether we are in devlopment mode. 
            {
                app.UseDeveloperExceptionPage(); // If we are in devlopment mode and our app encounters a problem, the  we use the UseDeveloperExceptionPage.
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(policy => policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200")); // We add this to allow “Cross origin resource sharing” to prevent the CORS error being triggered when using a http request between our client and our api. WE MUST ADD THIS BETWEEN UseRouting AND UseAuthorization. The parameter we pass in here is the policy we are going to use. We have to AllowAnyHeader, AllowAnyMethod (put requests, get requests etc) and specify the origins we want to allow using 'WithOrigins' which is: http://localhost:4200. What this says is that we will allow any header to be sent and any method to be used (get request put request etc) as long as it comes from the origin http://localhost:4200

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers(); // 
            });
        }
    }
}
