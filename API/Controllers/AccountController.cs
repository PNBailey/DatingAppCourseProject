using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// We create an account controller class here to handle to register http api request. This creates a user object when the end point is reached and adds the user to our database. It also creates a password hash and password salt using the password that is passed into the register method 

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AccountController(DataContext context, ITokenService tokenService, IMapper mapper) // We have to inject a dependancy to the DataContext as we want to get some data from the database from within this controller. With this private property above and this constructor with dependancy injection, we have access to the database via the DbContext just by using _context. We inject our token service as we want to generate a token for our user within this class 
        {
            _tokenService = tokenService;
            _mapper = mapper;
            _context = context;


        }

        [HttpPost("register")] // We use HttpPost when we want to add a new resource through our api endpoint. We add "register" here as this is what we will need to use on our client side http request end point to trigger this method
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        { // We use the ActionResult as this is the normal thing we return from any of our controller methods. This is an async method. We specify the type of thing we are returning, in this case an Appuser. 

            // When sending data using a Post request, we have to send the data as a object. We pass in our RegisterDto class object as the body of the request needs to have an object rather than string queries. This allows us to use the properties from this class to create our Appuser below. The Appsure we return here is what is sent to the database so therefore we cannot just assign string queries

            // The register method here needs some parameters that we are going to received as part of the post request. We can send data up in a post request in the body and it is going to be received by the [ApiController]. The [ApiController] attribute automatically binds to any parameters it finds in the parameters of our method here

            if (await UserExists(registerDto.Username)) // This tests whether the uername exists 

            {
                return BadRequest("Username is taken"); // We get access to the BadRequest object because we are using an ActionResult. the actionresult allows us to return diffetent http status codes as a response to this. The Badrequest is a 400 status error 

            }

            var user = _mapper.Map<AppUser>(registerDto);


            using var hmac = new HMACSHA512(); // This provides us with our hashing algorithm that we are going to use to create a password hash. The using statement ensures when we are finished with this class, it will be dispsed of correctly. Anytime we are using a class with this using statement, it's going to call a method inside this class called 'dispose', so that is disposes of this as it should do. Any class that implements the dispose method will implement something called, the Idisposable interface. Any class that implements the Idisposable interface, has to provide this dispose method. The using method takes care of all this, it automatically disposes of the class.

          
                user.Username = registerDto.Username.ToLower(); // We want our username to be unique in our database. We are going to use our username for many different things so we want it to be unique. We create a private helper method in this class. See UserExists method below...


                user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)); // This creates a password hash using the HMACSHA512 class. This method takes a byte array as a parameter. As the password we pass in to this register method is a string, we need to make this into a byte array. This is why we use the 'hmac.ComputeHash(Encoding.UTF8.GetBytes(password))' to convert the string password that is passed into the register method into a byte array. The PasswordHash property from the Appuser expects a byte array so it is fine to pass this to the PasswordHash field. 

                user.PasswordSalt = hmac.Key; // The hmac instance here is generated with a randomly generated key. Here we set the PasswordSalt to the randomly generated key that is created when the instance of HMACSHA512 is created 




            _context.Users.Add(user); // This tells entity framework that we want to add the user we created above to our users table. We are not actually adding anything here, all we are doing is telling entity framework to track this.

            await _context.SaveChangesAsync(); // This part we do actually call our database and we save our user into our users table.

            return new UserDto // We return the userDto so that we are able to access it when the method is called. This is the object that will be returned from our http Register post. We do this as we don;t want to receive the actual user object back as this includes the password etc. We also want to receive the token back as this contains the expiry time 
            {
                Username = user.Username, // we assign the user name to the users user name from the app user we create above
                Token = _tokenService.CreateToken(user), // 

                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }

        [HttpPost("login")]

        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto) // This is our login http async method. We pass in the loginDto DTO that we created as the body of the http post request will require an object rather than two strings (username and password)
        {
            var user = await _context.Users.

            Include(p => p.Photos)
            
            .SingleOrDefaultAsync(x => x.Username == loginDto.Username); // This will return the only element in users that matches our users details. If there is more than one elemt found with the details, it will return an exception. It will return a default value if the username is not found 

            if (user == null) return Unauthorized("Invalid Username"); // We test whether the default value is returned from the SingleOrDefaultAsync (null). If the default value is returned then this means the username was not found in our database. We return the Unauthorised 

            using var hmac = new HMACSHA512(user.PasswordSalt); // This hmac does the reverse of what we did previously. It takes the PasswordSalt and converts it back to the hash 

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password)); // This gets the hash from the password that is passed into the Login method above so that we can compare it below from the one in our database

            for (int i = 0; i < computedHash.Length; i++)  // This method loops over every 'byte' in our computedHash and PasswordHash byte array and checks to see if they match. If they do not match (so the password the user enters does not match the password from the database), the the message 'Invalid Password' is returned
            {
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password");
            }

                  return new UserDto // We return the userDto so that we are able to access it when the method is called. This is the object that will be returned from our http Register post. We do this as we don;t want to receive the actual user object back as this includes the password etc. We also want to receive the token back as this contains the expiry time 
            {
                Username = user.Username, // we assign the user name to the users user name from the app user we create above
                Token = _tokenService.CreateToken(user), // We get our token using the create token method in our token service file 
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url, // Using the ? optional assignment means that the PhotoUrl is nullable. This means that if there is no IsMain photo, null will be assigned to PhotoUrl.
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }

        private async Task<bool> UserExists(string userName) // This method checks if the userName already exists in our database
        {
            return await _context.Users.AnyAsync(x => x.Username == userName.ToLower()); // This method simply checks if the userName is found in our database. We convert the userName to lower case as when we send the username to the database, it will also be in lower case so it ensures it is comparing the strings in the same format
        }
    }
}