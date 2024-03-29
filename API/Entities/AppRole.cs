using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace API.Entities
{

    // We create this class because...
    public class AppRole : IdentityRole<int>
    {
        public ICollection<AppUserRole> UserRoles { get; set; }
    }
}