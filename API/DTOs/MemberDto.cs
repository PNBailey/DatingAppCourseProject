using System;
using System.Collections.Generic;
using API.Entities;
using API.Extensions;

namespace API.DTOs
{

    // We create this DTO to resolve the issue we had with circular referencing when receiving the photos for each user back from the http request. 
    public class MemberDto
    {

           public int Id { get; set; } // Entity framework uses conventions to do certain things. When we call this proprerty Id, Entity framework will recongnise that this is going to be our primary key and because it's an int, it's also going to setup our database so it automatically increments the Id field every time a new record is added to the database. Because we need entity to be able to get and set these properties and the entity itself, we use the public access modifier 
           public string UserName { get; set; }  

           public string PhotoUrl { get; set; }

           public int Age { get; set; } // Automapper can get the users age as we have a GetAge method in our Appuser entity. When we convert the type from appuser to member dto, the age is caulcated using the method in the appuser as this automapper is clever enough to do this  

           public string KnownAs { get; set; }

           public DateTime Created { get; set; } // We can initialise these properties. This is when the users profile was created 

           public DateTime LastActive { get; set; }

           public string Gender { get; set; }

            public string Introduction { get; set; }

           public string Interests { get; set; }

           public string City { get; set; }

           public string Country { get; set; }

           public ICollection<PhotoDto> Photos { get; set; } // This is the relationship between our AppUser entity and our Photo entity. This is a one to many relationship. 

    }
}
   