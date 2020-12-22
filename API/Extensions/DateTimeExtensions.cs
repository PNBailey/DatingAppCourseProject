using System;

namespace API.Extensions
{

    // We create an extension method here so that we can extend the GetAge method in our AppUsr entity. As there is no built in method to calculate someones age on a DateTime object. This is why we are extending this method to do this. We could have just done these calculations within the GetAge 
    public static class DateTimeExtensions
    {
        public static int CalculateAge(this DateTime dob) {
            var today = DateTime.Today;

            var age = today.Year - dob.Year;

            if(dob.Date > today.AddYears(-age)) age--;

            return age; 
        }
    }
}