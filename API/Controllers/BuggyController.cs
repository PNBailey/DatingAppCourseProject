using API.Controllers;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// We create a controller to handle our errors. We are going to create several different methods which are going to generate responses that are not successful 

namespace api
{
    public class BuggyController : BaseApiController
    {
        private readonly DataContext _context;
        public BuggyController(DataContext context)
        {
            _context = context;

        }

        [Authorize] 
        [HttpGet("auth")]

        public ActionResult<string> GetSecret() // This will test our 401 unauthorised responses
        
        {
            return "secret text";
        }

          [HttpGet("not-found")]

        public ActionResult<AppUser> GetNotFound() // This will test our 404 not found response 
        
        {
            var thing = _context.Users.Find(-1);

            if(thing == null) return NotFound(); 

            return Ok(thing);
        }

          [HttpGet("server-error")]

        public ActionResult<string> GetServerError() // This will test our 401 unauthorised responses
        
        {
            var thing = _context.Users.Find(-1); // This will return null

            var thingToReturn = thing.ToString();

            return thingToReturn;
        }

          [HttpGet("bad-request")]

        public ActionResult<string> GetBadRequest() // This will test our 401 unauthorised responses
        
        {
            return BadRequest("This was ot a good request");
        }
    }
}