using System;
using System.Collections.Generic;
using API.Extensions;

namespace API.Entities
{

    // This class will be one of the entities (one of the tables in our database).
    public class AppUser // The properties below are the columns we want in our database
    {
           public int Id { get; set; } // Entity framework uses conventions to do certain things. When we call this proprerty Id, Entity framework will recongnise that this is going to be our primary key and because it's an int, it's also going to setup our database so it automatically increments the Id field every time a new record is added to the database. Because we need entity to be able to get and set these properties and the entity itself, we use the public access modifier 
           public string UserName { get; set; }  

           public byte[] PasswordHash { get; set; } // we use byte array as this is what's going to be calculated when we return the hash. We will store this as byte arrays in our database (remember this isn't the best way to do this, this is just a very basic authentication method)

           public byte[] PasswordSalt { get; set; } // This is where we will store the password salty

           // To ensure the two properties get added as columns in our database, we must add them to the migration

           public DateTime DateOfBirth { get; set; }

           public string KnownAs { get; set; }

           public DateTime Created { get; set; } = DateTime.Now; // We can initialise these properties. This is when the users profile was created 

           public DateTime LastActive { get; set; } = DateTime.Now;

           public string Gender { get; set; }

           public string Introduction { get; set; }

           public string Interests { get; set; }

           public string City { get; set; }

           public string Country { get; set; }

           public ICollection<Photo> Photos { get; set; } // This is the relationship between our AppUser entity and our Photo entity. This is a one to many relationship

           public ICollection<UserLike> LikedByUsers { get; set; } // We add this to enable the many to many relationship needed for our UserLike entity. This is the users who have liked the curretly logged in user

           public ICollection<UserLike> LikedUsers { get; set; } // This is the users that the currently logged in user has liked 
    }
}