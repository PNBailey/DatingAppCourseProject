using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        private readonly IPhotoService _photoService;

        private readonly IUserRepository _userRepository;
        public UsersController(IUserRepository userRepository, IMapper mapper, IPhotoService photoService) // We have to inject a dependancy to the DataContext as we want to get some data from the database from within this controller. With this private property above and this constructor with dependancy injection, we have access to the database via the DbContext just by using _context
        {
            _photoService = photoService;
            _mapper = mapper;
            _userRepository = userRepository;

        }

        // We add two endpoints here. 1 to get all of the users in our database and another 1 to get specific users 

        // If WE ARE MAKING DATABASE CALLS, ALWAYS MAKE THE CODE ASYNCHRONOUS 
        // [AllowAnonymous]

        [Authorize(Roles = "Admin")] // Adding this attribute means that only users within the Admin role are able to call the method 

        [HttpGet] //We use this attribute when we want to get data from the database 
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers([FromQuery]UserParams userParams) // Using the [FromQuery] attribute will mean that the query string will be retrieved from the userParams object we passed in. The [FromQuery] attribute finds the required values from the URL of the browser. So when a user submits a request to get the users, the useerParams properties (which includes page number, page size etc) will be found in the URL. In order to retrieve those values from the URL, we must specify the [FromQuery] attribute The api controller isn't smart enough to retrieve the query strings (page size etc) from the userParams object without this
        { // We specify the type that we want to get back from the request. We use the collections type Ienumerable and the type of collection we want here is the MemberDto types. Ienumerable allows us to use simple iteration over a collection of a specified type. We could have used the 'List' type here instead. This is an async method as we would need to wait for the data to come back and allow other code to execute. This makes the app more scalable 

        var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

            userParams.CurrentUserName = user.UserName;

            if(string.IsNullOrEmpty(userParams.Gender))
                userParams.Gender = user.Gender == "male" ? "female" : "male";

            var users = await _userRepository.GetMembersAsync(userParams);

            Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages); // We always have access to our request http response in our controllers. 

            return Ok(users);

            // Rather than do all the below where we map each user to a member Dto using automapper locally, we use the GetMembersAsync method which maps each user at the database level as this is more efficient.    

            // var users = await _userRepository.GetUsersAsync(); // We use the linq method 'ToListAsync' here to convert the Users data from our database to a list. We have to use the async version of this method so that other code can execute whilst this gets the data in the background. When the request now goes to the database, it pauses and waits. It defers it to a 'Task' that then goes to the query to the database. When the task comes back, we need to get the results out of the task and we do that by using the await keyword

            // var usersToReturn = _mapper.Map<IEnumerable<MemberDto>>(users); // As we created our autoMapper Create Maps, we can use the automapper here to convert the type from Appusers to MemberDto's. This is to prevent the circular reference issue we had with the users photos

            // return Ok(usersToReturn); // This prevents a type error with this method as this returns an actio result but the method in the inherited interface did not return an action result 

        }

        // [Authorize] // This ensures our end point is protected and that the users can only be retrieved if there is a valid user token (so if the user is actaully logged in)
        [Authorize(Roles = "Member")]
        [HttpGet("{username}", Name = "GetUser")] //We use this attribute when we want to get data from the database. As we only want to find one user here and we want to find the uswer by there id, we specify a route parameter. When the client/user hits this endpoint what they are doing is going to api/users/2. We set the name to GetUser so that we can use the route name in our add photo method below
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        { // We pass in the id that we et from the rout parameter 

            return await _userRepository.GetMemberAsync(username); // The GetMemberAsync methid we created will return a Member Dto directly from the repository

            // var user = await _userRepository.GetUserByUsernameAsync(username);
            // return _mapper.Map<MemberDto>(user); // We use the method find here as we pass in the primary key to find the correct user from the Users table in our database. So when angular routes to this url (endpoint), the url will be api/users/2 (2 is just ane example id). This endpoint will then be triggered and the user will be found in the database and returned


        }

        [HttpPut] // The HttpPut will allow us to update a respurce on our server. As the HttpPut different to the HttpGet, we can use the same end point as the HttpGet above (which is the default /users endpoint)

        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        { // As the client already has the information we are updating and we don't need to receive anything back from the http request, we don't provide a return type like we do in the other methods above



            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername()); // We need to get hold of the user and the users username. We don't want to trust the user to manually input their username when updating their profile. We want to get it from what we are authenticating against which is the token. Inside a controller we have access to a claims principle of the User. This contains information about their identity. The token contains this username as we specified this in our token service file. The 'User' claims principle is accessible in the controllers. See Claims principle extension method file 

            _mapper.Map(memberUpdateDto, user); // When we using mapper to update an object, we can use the Map method. So this will prevent us from manually maping our memberUpdateDto to our user object and it will handle this for this us

            _userRepository.Update(user); // This update method will add a flag to our user object to say that this object has been updated by entity framework. This guarantees that we are not going to get an exception or an error when we update the user in our database

            if (await _userRepository.SaveAllAsync()) return NoContent(); // We use the NoContent when we don't need to receive any data back from the http request. The SaveAllAsync method returns either 1 or -1, hence we can use it in a conditional statement 

            return BadRequest("Falied to update user");

        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)// We need pass back the id of the photo and whether the photo is the main photo 
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername()); // The GetUserByUsernameAsync method includes our users photos as we are eagerly loading the photos with the user as we used the 'Include' method in our userRepository 

            var result = await _photoService.AddPhotoAsync(file);

            if(result.Error != null) return BadRequest(result.Error.Message); // If the request doesn't work, there will be an error property attached to the result object we created above. The error message here will be coming from cloudinary

            var photo = new Photo 
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            if (user.Photos.Count == 0)  
            {
                photo.IsMain = true; // This will set the users photo to their main photo if they don't have any other photos 
            }

            user.Photos.Add(photo); // This adds a flag to the user to say changes have been made so that when we use our saveall async method below, the user is updated in our database 

            if(await _userRepository.SaveAllAsync()) {
                // return _mapper.Map<Photo, PhotoDto>(photo); // We map our photo to our photo dto as we only want to return our Photo Dto properties rather than our full Photo object  

                return CreatedAtRoute("GetUser", new {username = user.UserName}, _mapper.Map<Photo, PhotoDto>(photo)); // The correct response type from the server when a resource is created (when a photo is added for example) is a 201 response. The 201 response includes a location header. To manually set a 201 response type we use the Created keyword. The "GetUser" is the route name we speficied in our GetUser endpoint. We specify this route as this is how we get the user (this is the only way to get the photos) which contains the photos. We provide the username as an object as this is what the GetUser endpoint expects. Using this CreatedAtRoute means that when the 201 response comes back, it is able to get the location of the photo. So the location will be "https://localhost:5001/api/Users/cecilia" for example. So the response that the client receives will now include the locatio of the image as we have amended the type of response to a 201 which includes the location in the header

            }
                


            return BadRequest("Problem adding photo");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId) 
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId); // We get the photo from the users photo array using the photo id

            if(photo.IsMain) return BadRequest("This is already your main photo"); // We check to see whether the photo is already the users main photo

            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain); // This gets the photo from the users photos array where the photos isMain property is set to true. This: "FirstOrDefault(x => x.IsMain);" is the shorthand for FirstOrDefault(x => x.IsMain == true);

            if(currentMain != null) currentMain.IsMain = false;

            photo.IsMain = true;

            if(await _userRepository.SaveAllAsync()) return NoContent(); // as we don't need to send anything back from this request, we use the NoContent. We only do this if the saveallasync method returns > 0 (returns true) as this tells us it was successful 

            return BadRequest("Failed to set main photo");
            
        }

        [HttpDelete("delete-photo/{photoId}")]

        public async Task<ActionResult> DeletePhoto(int photoId) 
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if(photo == null) return NotFound();

            if(photo.IsMain) return BadRequest("You cannot delete your main photo");

            if(photo.PublicId != null) // We check to see whether the photo has a public Id as if it doesn't then we don't need to delete it from cloudinary 
            {
                var result = await _photoService.DeletionPhotoAsync(photo.PublicId);
                if(result.Error != null) return BadRequest(result.Error.Message);
            } 

            user.Photos.Remove(photo); // This adds the tracking flag to the entity so that when the savechanges async is used below, then the database is updated correctly 

            if(await _userRepository.SaveAllAsync()) return Ok(); // We use the SaveAllAsync method from the user repository as the photos are relating to the user. 

            return BadRequest("Failed to delete the photo");

            
        }

    }
}