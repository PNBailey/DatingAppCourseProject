using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// A CONTROLER IS PART OF THE MVC DESIGN PATTERN WE USE WITHIN PROGRAMMING. THE VIEW WILL BE THE CLIENT (ANGULAR WILL BE USED TO PROVIDE THE VIEW IN THIS APP). 

namespace API.Controllers
{
    [ApiController] // This signifies that this class is of type API controller. There are certain things this adds to our controller...
    [Route("[controller]")] // This 'controller' in square brackets is a placeholder. This gets replaced with the first part of the controller name (weatehrForcast in this case).  This defines the route in which the clinet is going to get to the API controller 
    public class WeatherForecastController : ControllerBase // A controller needs to derive from a controller base.
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
