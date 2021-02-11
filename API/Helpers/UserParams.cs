namespace API.Helpers
{

    // We inherit from the PaginationParams class so that we still get acess to the PaginationParams properties
    public class UserParams : PaginationParams
    {
        public string CurrentUserName { get; set; }

        public string Gender { get; set; }

        public int MinAge { get; set; } = 18;

        public int MaxAge { get; set; } = 150;

        public string OrderBy { get; set; } = "lastActive"; 


    }
}