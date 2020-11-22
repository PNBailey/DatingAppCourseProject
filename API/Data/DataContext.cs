using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data // This should match the folder paths 
{
    public class DataContext : DbContext // We derive the DataContext class from the parent DbContext class This comes with entity framework. The DbContext class acts a bridge between our domain (or entity classes) and the database. This Dbcontext class is the primary class we use for interacting with our database.

// Entity framework allows us to write something called LINQ queries. When we add an entity (Users in our app for e.g.) to our Dbcontext class we have access to a property called Users which represents our users table in our database and then we can use certain methods on this property such as Users.add. See below:

    {

        public DataContext(DbContextOptions options)
            : base(options)
        {
            
        }

        public DbSet<AppUser> Users { get; set; } // We add a DbSet. We pass in the type that we want to create a database set for (in this case Users). The Users we specify here is the table we are calling. We then need to add this configuration to our starter class so we can inject the data context into other parts of our app 
        
    }
}