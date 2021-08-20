using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace API.Entities
{

    // This class will be one of the entities (one of the tables in our database).
    public class AppUser : IdentityUser<int>// The properties below are the columns we want in our database. By Inheriting from IdentityUser, we are able to use the features of .NET Identity framework. We spcecify the type int here as we want the primary key of the ap user entity to be the Id which is an int
    {
        //    public int Id { get; set; } // Entity framework uses conventions to do certain things. When we call this proprerty Id, Entity framework will recongnise that this is going to be our primary key and because it's an int, it's also going to setup our database so it automatically increments the Id field every time a new record is added to the database. Because we need entity to be able to get and set these properties and the entity itself, we use the public access modifier. As we are using .NET Identity framework, we no longer have to specify this field as Identity framework comes with this field

        //    public string Username { get; set; } As we are using .NET Identity framework, we no longer have to specify this field as Identity framework comes with this field

        //    public byte[] PasswordHash { get; set; } // we use byte array as this is what's going to be calculated when we return the hash. We will store this as byte arrays in our database (remember this isn't the best way to do this, this is just a very basic authentication method).As we are using .NET Identity framework, we no longer have to specify this field as Identity framework comes with this field

        //    public byte[] PasswordSalt { get; set; } // This is where we will store the password salt. As we are using .NET Identity framework, we no longer have to specify this field as Identity framework comes with this field

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

           public ICollection<Photo> Photos { get; set; } // This is the relationship between our AppUser entity and our Photo entity. This is a one to many relationship. Using the Icollection interface gives us access to methods like .Add and .Contains on the Photos

           public ICollection<UserLike> LikedByUsers { get; set; } // We add this to enable the many to many relationship needed for our UserLike entity. This is the users who have liked the curretly logged in user. Using the Icollection interface gives us access to methods like .Add and .Contains on the LikedbyUsers

           public ICollection<UserLike> LikedUsers { get; set; } // This is the users that the currently logged in user has liked. Using the Icollection interface gives us access to methods like .Add and .Contains on the Likedusers

            public ICollection<Message> MessagesSent { get; set; }

            public ICollection<Message> MessagesReceived { get; set; }

             public ICollection<AppUserRole> UserRoles { get; set; }
    }
}