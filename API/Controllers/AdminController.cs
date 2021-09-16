using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AdminController : BaseApiController
    {
        public UserManager<AppUser> _userManager { get; }
        public AdminController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        [Authorize(Policy = "RequireAdminRole")] // This is how we use Policy based authorisation in Identity
        [HttpGet("users-with-roles")]
        public async Task<ActionResult>  GetUsersWithRoles() // As we want the admins to be able to amend peoples roles, we need a method to get a user with their roles. This way, we can amend their roles
        {
            var users = await _userManager.Users
                .Include(r => r.UserRoles)
                .ThenInclude(r => r.Role)
                .OrderBy(u => u.UserName)
                .Select(u => new 
                { // Here we return an annonymous object. So we will get an object back with the users Id, their username, ad the roles that they're in
                    u.Id,
                    UserName = u.UserName,
                    Roles = u.UserRoles.Select(r => r.Role.Name).ToList()
                })
                .ToListAsync(); // We then send this to a list

            return Ok(users);
        }

        [HttpPost("edit-roles/{username}")] // we add a route parameter here called username

        public async Task<ActionResult> EditRoles(string username, [FromQuery] string roles) // We get the roles from a query string

        {
            var selectedRoles = roles.Split(",").ToArray(); // This takes the roles from the query string, splits them using a comma to seperate them ad converts them to an array

            var user = await _userManager.FindByNameAsync(username); // This gets the user in the database 

            if(user == null) return NotFound("Could not find user");

            var userRoles = await _userManager.GetRolesAsync(user); // This gets the roles that the above user has assigned to them 

            var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles)); // This adds the roles to the user that is passed over from the client but does not add the roles the user is already in. This is achieved by using the .Except method which is a method of the IEnumerable type

            if(!result.Succeeded) return BadRequest("Failed to add to roles");

            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles)); // We also want to remove the roles the user was previously in as we have now defined the new roles the user is in

            if (!result.Succeeded) return BadRequest("Failed to remove from roles");

            return Ok(await _userManager.GetRolesAsync(user)); // This returns the new roles the user is assigned to
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("photos-to-moderate")]
        public ActionResult GetPhotosForModeration()
        {
            return Ok("Admins or moderators can see this");
        }


    }
}