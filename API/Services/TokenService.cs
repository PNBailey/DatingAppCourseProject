using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

// To accommodate the concept of single responsibility principle, we need to create a service that is solely responsible for the creation of JWT tokens. It’s not going to issue the tokens, that’s going to be the job of the account controller. This service is simply going to receive a user (AppUser in our project) and it’s going to create a token for that user and return it to the account controller. 

// **** Once we have created this class, we need to add it to our ConfigureServices method within our startup class as this is where we inject our dependancies ****

namespace API.Services
{
    public class TokenService : ITokenService
    {

        private readonly SymmetricSecurityKey _key; // This is where we will store our key. Symmetric encrytion is a type of encrytion when only one key is used to both encrypt and decypt electronic information. The same key is used to sign our token and make sure our signituare is verified. The other type is Asymetric encryption where there is a pair of keys.
        public UserManager<AppUser> UserManager { get; }

        public TokenService(IConfiguration config, UserManager<AppUser> userManager) // We add a constructor here as we are going to inject our configuration into this class. As we are nw using identity, we need a safe place to store what roles the user belongs to when they register/log in. Adding the roles that a user belongs to, to the JWT token is a good idea as the user will not be able to modify the roles they are assigned to, to trick the server.
        {
            UserManager = userManager;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"])); // We create a ey from our Token key and we convert it into a byte array as this is what type the SymmetricSecurityKey needs to be. **** When we add this, we need to add it to our appsettings.Development.JSON file
        }
        public async Task<string> CreateToken(AppUser user) // A token can take claims, credentials and other information. As we are using the await keyword within this method, we need to make the method async and we need to return a Task instead 
        {
            var claims = new List<Claim>  // We start off by identifying what claims we are going to put inside of this token. 
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()), // This will be our name identifier for just about everything. We use the NameId do the users ID. We have to use the ToString method on the Id as claims principles have to be strings 

                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName) //We use the UniqueName field to store the user.userName.
            };

            var roles = await UserManager.GetRolesAsync(user); // this gets a list of the roles that the user is assigned to

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role))); // Here we select the role from the list of roles, we then create a new claim and rather than use JWT claim naes, we use claim types because the claim names do not have an option for role. 

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature); // This object takes a security key and an algorithm 

            // Now we need to describe our token:

            var tokenDescriptor = new SecurityTokenDescriptor // We specify here what goes inside of our token. This descrbes how this token is going to look
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);

            // This is a lot of code here but ultimately all we need to know is that this is the code that will create our token. When we want to create a new token within a class, we inject this service into the class. We can then use the create token method which allows us to create a new token based on the user


        }
    }
}