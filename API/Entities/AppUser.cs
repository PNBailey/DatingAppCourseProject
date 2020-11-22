namespace API.Entities
{

    // This class will be one of the entities (one of the tables in our database).
    public class AppUser
    {
           public int Id { get; set; } // Entity framework uses conventions to do certain things. When we call this proprerty Id, Entity framework will recongnise that this is going to be our primary key and because it's an int, it's also going to setup our database so it automatically increments the Id field every time a new record is added to the database. Because we need entity to be able to get and set these properties and the entity itself, we use the public access modifier 
           public string UserName { get; set; }  
    }
}