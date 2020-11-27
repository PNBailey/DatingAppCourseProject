namespace API.DTOs
{

    // This is the object we are going to return when the user registers on the app. We don't want the API to return the actual user as this will return the password hash details etc.
    public class UserDto
    {
        public string UserName { get; set; }

        public string Token { get; set; }
    }
}