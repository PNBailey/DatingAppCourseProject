using System.Text.Json;
using API.Helpers;
using Microsoft.AspNetCore.Http;

namespace API.Extensions

// We create this extension method so that we can add a pagination header to a http response

{
    public static class HttpExtensions
    {
        public static void AddPaginationHeader(this HttpResponse response, int currentPage, int itemsPerPage, int totalItems, int totalPages)  // We are extending ghe HttpResponse base class 
        {
            var paginationHeader = new PaginationHeader(currentPage, itemsPerPage, totalItems, totalPages);

            var options = new JsonSerializerOptions { // This changes the header response json data to camel case. We pass these options to our serialize method below
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
            };

            response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationHeader, options)); // This adds the header to the response. We have to serialize this because when we add this our response headers take a key and string value. So the ""Pagination" will be our key and our JsonSerializer.Serialize(paginationHeader) is our value. 

            response.Headers.Add("Access-Control-Expose-Headers", "Pagination"); // When adding a custom header, we need toa dd a CORS header onto this to make this header available. The second argument here is the name of the header we are 'exposing'. In this case it i our custom "Pagination" header. 
        }
    }
}