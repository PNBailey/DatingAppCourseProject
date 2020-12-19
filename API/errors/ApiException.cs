namespace API.errors
{

    // We create this class to handle our api exceptions globally. We also need to create our exceptions middleware. See Exceptionsmiddleware class 
    public class ApiException
    {
        public ApiException(int statusCode, string message = null, string details = null) // The message and details will be set to null if no value is given when an instance of this class is created 
        {
            StatusCode = statusCode;
            Message = message;
            Details = details;
        }

        public int StatusCode { get; set; }

        public string Message { get; set; }

        public string Details { get; set; }


    }
}