using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data // This should match the folder paths 
{
    public class DataContext : IdentityDbContext<
    AppUser, 
    AppRole, 
    int, 
    IdentityUserClaim<int>, 
    AppUserRole, 
    IdentityUserLogin<int>,
    IdentityRoleClaim<int>,
    IdentityUserToken<int>> // We derive the DataContext class from the parent DbContext class This comes with entity framework. The DbContext class acts a bridge between our domain (or entity classes) and the database. This Dbcontext class is the primary class we use for interacting with our database. ** To get access to the IdentityDBContext, we need to install the Entity framework package called 'Microsoft.AspNetCore.Identity.EntityFrameworkCore' from the NuGet gallery. Because we want to access the user roles and we've given our entities a different key (we're going to be using int instead or strings), we need to provide type parameters.

// Entity framework allows us to write something called LINQ queries. When we add an entity (Users in our app for e.g.) to our Dbcontext class we have access to a property called Users which represents our users table in our database and then we can use certain methods on this property such as Users.add. See below:

    {

        public DataContext(DbContextOptions options)
            : base(options)
        {
            
        }

        // public DbSet<AppUser> Users { get; set; } // We add a DbSet. We pass in the type that we want to create a database set for (in this case Users). The Users we specify here is the table we are calling. We then need to add this configuration to our starter class so we can inject the data context into other parts of our app. *** The Identity DBContext provides us with the tables we need to populate our database with identity so we don't need to provide this

        public DbSet<UserLike> Likes { get; set; }

        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder) // As we need to do something with our Likes, we need to do some configuration. Here we are overriding the DbContext method 'OnModelCreating'. Using override allows us to override an inhereted method 
        {
            base.OnModelCreating(builder); // If we don;t do this, we can sometimes get errors when we try and add a migration. We get access to the 'base' when we are overriding a method

            builder.Entity<AppUser>() // Here we configure the many to many relationship betwee our AppUser and AppRole entities
                .HasMany(ur => ur.UserRoles)
                .WithOne(u => u.User)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();

            builder.Entity<AppRole>() // Here we configure the many to many relationship betwee our AppUser and AppRole entities
                .HasMany(ur => ur.UserRoles)
                .WithOne(u => u.Role)
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired();


            builder.Entity<UserLike>() // This allows us to 'work' on our entity here. We pass in the entity as a type parameter of the entity we want to configure 
                .HasKey(key => new {key.SourceUserId, key.LikedUserId}); // Because we didn't identify a primary key for this particular entity, we are going to configure this key ourself. It's going to be a combination of the source user id and the liked user id. So if the SourceUserId (the one that has done the 'liking') is '27' and the LikedUserId is '34', then the like is will be '27 34'

            builder.Entity<UserLike>() // This configures the relationships
                .HasOne(s => s.SourceUser) // A source user can like many other users is what we are saying/configuring here
                .WithMany(l => l.LikedUsers)
                .HasForeignKey(s => s.SourceUserId)
                .OnDelete(DeleteBehavior.Cascade); // This means that is we delete the user, we delete the related entities (i.e. if we delete a user, we delete their likes)

            builder.Entity<UserLike>() // This configures the relationships
                .HasOne(s => s.LikedUser) // A liked user can be liked by many other users (source users , see above) is what we are saying/configuring here
                .WithMany(l => l.LikedByUsers)
                .HasForeignKey(s => s.LikedUserId)
                .OnDelete(DeleteBehavior.Cascade); // This means that is we delete the user, we delete the related entities (i.e. if we delete a user, we delete their likes)

            builder.Entity<Message>()
                .HasOne(u => u.Recipient)
                .WithMany(m => m.MessagesReceived)
                .OnDelete(DeleteBehavior.Restrict); // Using Restrict means that the messages will not be removed if the other party hasn't deleted them themselves

            builder.Entity<Message>()
                .HasOne(u => u.Sender)
                .WithMany(m => m.MessagesSent)
                .OnDelete(DeleteBehavior.Restrict);
             
        }

        
        
    }
}