using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{

    // We create this class so that we can pass this type into the our http post request as the body of this request must be an object rather than the strings below
    public class RegisterDto
    {

        [Required] // To prevent blank usernames etc, we need to add validation. We could add the data validation at many different levels on our app (in the appsure class, entity class etc) but we will add it in our DTO’s as these are the properties we are receiving in the body of our request. The api controller contains some automated validation checks that we can utilise. It automatically validates the parameters that we pass up to an API endpoint based on the validation we set. To use validation. In this DTO class, we use the [Required] data annotation to ensure this value is actually used. If it is not used or it is an empty string, it returns a 400 error with the error message "The username field is required"

        public string UserName { get; set; }

        [Required] // There are many different types of validation such as email address validation, phone number validation etc 

        [StringLength(8, MinimumLength = 4)] // Thi validator ensures that at least 4 charcters and max 8 charcters are enetered by the user for the password 
        public string Password { get; set; }
        
    }
}