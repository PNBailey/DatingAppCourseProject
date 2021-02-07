namespace API.Entities
{

    // As self referencing for many to many relationships in entity framework is so new, it does not work properly Therefre we have to create a join table to connect the user that is doing the liking and the user that ie being liked. This is why we are creating this separate entity here. ONCE THIS IS ADDED, WE NEED TO CONFIGURE OUR DATACONTEXT FILE AND WE NEED TO RE-RUN THE MIGRATION
    public class UserLike
    {
        public AppUser SourceUser { get; set; } // This is the user that is liking the other user
        public int SourceUserId { get; set; } 

        public AppUser LikedUser { get; set; }

        public int LikedUserId { get; set; }
    }
}