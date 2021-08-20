using Microsoft.AspNetCore.Identity;

namespace API.Entities
{

    //This is our join table. It connects the many to many relationship between our AppUser and our AppRole (a user can have many roles and role can have many users)
    public class AppUserRole : IdentityUserRole<int>
    {
        public AppUser User { get; set; }

        public AppRole Role { get; set; }
    }
}