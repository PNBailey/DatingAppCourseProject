
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;

namespace API.Data
{
    public class Seed
    {
        public static async Task SeedUsers(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager) // As we are now using identity within .NET we get access to the user manager so we can manage our users. As we want to seed the roles for the users, we inject the role manager into the method
        {
            if(await userManager.Users.AnyAsync()) return; // As we are now using Identity, we can access the Users via the userManager. This returns an iqueryable so we can query the database like we did using the DataContext.

            var userData = await System.IO.File.ReadAllTextAsync("Data/UserSeedData.json"); // This gets the text from the json file we created 

            var users = JsonSerializer.Deserialize<List<AppUser>>(userData); // This deserializes the json data to an object of whatever type we specify here. As the Json data properties match the format of our AppUser properties, it converts it correctly to a list of Appusers

            if(users == null) return;

            var roles = new List<AppRole> // Here we create a list which is expecting a list of AppRoles. We intialise the list and add 3 new AppRoles and assign values to there name properties
            {
                new AppRole{Name = "Member"},
                new AppRole{Name = "Admin"},
                new AppRole{Name = "Moderator"}
            };

            foreach(var role in roles) 
            {
                await roleManager.CreateAsync(role);
            }

            foreach(var user in users) {

                user.UserName = user.UserName.ToLower();

                // user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd")); *** As we are now using Identit Framework, we no longer need to create the Password Hash

                // user.PasswordSalt = hmac.Key; *** As we are now using Identit Framework, we no longer need to create the Password Hash


                // PREVIOUS CODE

                // await context.Users.AddAsync(user); // This adds tracking to the users through entity framework. This isn't actually adding the user. This is why we don't use the await keyword here. As we are now using Identity, we create the users using the code below.

                // NEW CODE

                await userManager.CreateAsync(user, "Pa$$w0rd"); // Using this method on the userManager takes the user we are creating as an AppUser and we can also add a password as the 2nd parameter. If we wanted to allow the creation of simpler, less secure passwords, we could add some further configuration in our startup class

                await userManager.AddToRoleAsync(user, "Member"); // This then adds the user within this for loop and assigns them to the member role we created within Idetities RoleManager       
            }

            var admin = new AppUser // Here we are creating a new AppUser so that we can assign them to be an admin 
            {
                UserName = "admin"
            };

            await userManager.CreateAsync(admin, "Pa$$w0rd");

            await userManager.AddToRolesAsync(admin, new[] {"Admin", "Moderator"}); // Here we are assigning multiple roles to the admin user we do that by creating a new array and initialising the array with two strings. This works as the 'AddToRolesAsync' method is expecting an IEnumerable of strings as it's 2nd argument

            // await userManager.SaveChangesAsync(); // The UserManager takes care of saving the changes so we don't need to worry about doing this
        }
    }
}
 