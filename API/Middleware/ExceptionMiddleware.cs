using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using API.errors;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace API.Middleware
{

    // When adding middleware into our api, we need a constructor with a requestdelegate
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env) // The request delegate is what's coming up next in the middleware pipeline. We add the ILogger so we can log the exception on the terminal and we give it a type of exceptionmiddleware. We also want to check what mode we are in (devlopment, prduction etc) so we add the IHostEnvironment to check the environment we are in
        {
            _env = env;
            _logger = logger;
            _next = next;

        }

        public async Task InvokeAsync(HttpContext context) // This is happening in the context of the http request. When we add middleware we have access to the actual http request. 

        {
            try {
                await _next(context); // Here we get our context and simply pass this onto the next piece of middleware. So if there is no exception error, the http response simply gets executed and the result is successfully returned
            }

            catch (Exception ex)
            
             {
                _logger.LogError(ex, ex.Message); // If we don't do this, our exception is going to be silent in our terminal
                context.Response.ContentType = "application/json"; // Here we write out our exception to the response. 
                context.Response.StatusCode = (int) HttpStatusCode.InternalServerError; // We cast this to an INT. This gives us the error status code

                var response = _env.IsDevelopment() ? new ApiException(context.Response.StatusCode, ex.Message, ex.StackTrace?.ToString()) : new ApiException(context.Response.StatusCode, "Internal Server Error");// We create an instance of the Apiexception class we created only when we are in devlopment mode The ? allows us to set the type to null

                var options = new JsonSerializerOptions{PropertyNamingPolicy = JsonNamingPolicy.CamelCase}; // This ensures our response goes back as normal JSON formatted response in camel case

                var json = JsonSerializer.Serialize(response, options); // This serialises the response we created and we pass in the options we created above 

                await context.Response.WriteAsync(json); 

                // All this code above is our error handling middleware. WE THE  NEED TO ADD THIS MIDDLEWARE TO OUR STARTUP CLASS AT THE TOP OF THE CONFIGURE METHOD
            }
        }

    }
}