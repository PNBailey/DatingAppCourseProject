using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities
{

    [Table("Photos")] // When entity framework creates this table, it will be called Photos. We don't add this to the DBContext derived class but the table will still be added to the database when we migrate as we have used this attribute
    public class Photo
    {
        public int Id { get; set; }

        public string Url { get; set; }

        public bool IsMain { get; set; }

        public string PublicId { get; set; } // We need this property for the photo storage solution we are going to use 


        // FULLY DEFINING RELATIONSHIP BETWEEN APPUSER AND PHOTO ENTITY:
        public AppUser AppUser { get; set; } // If we want to change the setup of the table so that we can prevent the AppUser auto created column to not be null for example, we delete the migrations and we ‘fully define the relationship’. To do this, we need to add this property 

        public int AppUserId { get; set; } // In relation to the above, we need to manually add the AppUserId field and this will be the link between the AppUser (user table) entity and the Photo Entity

        // ONCE THE ABOVE FULLY DEFINED RELATIONSHIP HAS BEEN CREATED, WE RUN THE MIGRATION

    }
}