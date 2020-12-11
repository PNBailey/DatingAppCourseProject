using Microsoft.AspNetCore.Mvc;

// We create a base class here so that we can derive other controller classes from it. This implements inheritance and DRY principles 

// Controllers always have 3 things:

namespace API.Controllers
{
    
    [ApiController] // 1. We have to add this attribute to our API controllers
    [Route("api/[controller]")] // 2. We also have to specify the route. 
    public class BaseApiController : ControllerBase // 3. controllers always inherit from the ControllerBase class 
    {
        
    }
}