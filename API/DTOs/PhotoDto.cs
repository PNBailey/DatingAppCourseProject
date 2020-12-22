namespace API.DTOs
{

    // Adding this DTO resolves the issue we had with circular referencing
    public class PhotoDto
    {

        public int Id { get; set; }

        public string url { get; set; }

        public bool IsMain { get; set; }

        

    }
}