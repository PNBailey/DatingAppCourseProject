using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// A CONTROLER IS PART OF THE MVC DESIGN PATTERN WE USE WITHIN PROGRAMMING. THE VIEW WILL BE THE CLIENT (ANGULAR WILL BE USED TO PROVIDE THE VIEW IN THIS APP). 

namespace API.Controllers
{
    // Controllers always have 3 things:
    // [ApiController] // 1. We have to add this attribute to our controllers
    // [Route("api/[controller]")] // 2. We also have to specify the route. If the client wishes to use this controller, it will need to specify 'api/Users'

    [Authorize] // Adding this attribute means that all the methods in this controller are protected by authorisation
    public class UsersController : BaseApiController // : ControllerBase // 3. controllers always inherit from the ControllerBase class 


    // AS WE ARE INHERITING FROM THE BASEAPICONTROLLER CLASS THAT WE CREATED AS A BASE CLASS, WE DO NOT NEED TO ADD THE [ApiController] ATTRIBUTE, THE [Route("api/[controller]")] OR THE BASEAPIRCONTROLLER IMPLEMENTAION ABOVE AS THE BASE CONTROLLER ALREADY HAS THIS
    {
        private readonly IMapper _mapper;

        private readonly IUserRepository _userRepository;
        public UsersController(IUserRepository userRepository, IMapper mapper) // We have to inject a dependancy to the DataContext as we want to get some data from the database from within this controller. With this private property above and this constructor with dependancy injection, we have access to the database via the DbContext just by using _context
        {
            _mapper = mapper;
            _userRepository = userRepository;

        }

        // We add two endpoints here. 1 to get all of the users in our database and another 1 to get specific users 

        // If WE ARE MAKING DATABASE CALLS, ALWAYS MAKE THE CODE ASYNCHRONOUS 
        // [AllowAnonymous]

        [HttpGet] //We use this attribute when we want to get data from the database 
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
        { // We specify the type that we want to get back from the request. We use the collections type Ienumerable and the type of collection we want here is the AppUser type we created in our AppUser Entity class. Ienumerable allows us to use simple iteration over a collection of a specified type. We could have used the 'List' type here instead. This is an async method as we would need to wait for the data to come back and allow other code to execute. This makes the app more scalable 



            var users = await _userRepository.GetMembersAsync();

            return Ok(users);

            // Rather than do all the below where we map each user to a member Dto using automapper locally, we use the GetMembersAsync method which maps each user at the database level as this is more efficient.    

           // var users = await _userRepository.GetUsersAsync(); // We use the linq method 'ToListAsync' here to convert the Users data from our database to a list. We have to use the async version of this method so that other code can execute whilst this gets the data in the background. When the request now goes to the database, it pauses and waits. It defers it to a 'Task' that then goes to the query to the database. When the task comes back, we need to get the results out of the task and we do that by using the await keyword

           // var usersToReturn = _mapper.Map<IEnumerable<MemberDto>>(users); // As we created our autoMapper Create Maps, we can use the automapper here to convert the type from Appusers to MemberDto's. This is to prevent the circular reference issue we had with the users photos

            // return Ok(usersToReturn); // This prevents a type error with this method as this returns an actio result but the method in the inherited interface did not return an action result 

        }

        // [Authorize] // This ensures our end point is protected and that the users can only be retrieved if there is a valid user token (so if the user is actaully logged in)

        [HttpGet("{username}")] //We use this attribute when we want to get data from the database. As we only want to find one user here and we want to find the uswer by there id, we specify a route parameter. When the client/user hits this endpoint what they are doing is going to api/users/2
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        { // We pass in the id that we et from the rout parameter 

            return await _userRepository.GetMemberAsync(username); // The GetMemberAsync methid we created will return a Member Dto directly from the repository

            // var user = await _userRepository.GetUserByUsernameAsync(username);
            // return _mapper.Map<MemberDto>(user); // We use the method find here as we pass in the primary key to find the correct user from the Users table in our database. So when angular routes to this url (endpoint), the url will be api/users/2 (2 is just ane example id). This endpoint will then be triggered and the user will be found in the database and returned


        }
    }
}