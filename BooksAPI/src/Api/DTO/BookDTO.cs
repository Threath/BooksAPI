namespace BooksAPI.src.Api.DTO
{
    public class BookDTO
    {
        public string? Title { get; set; }

        public string? Description { get; set; }

        public decimal? Rating { get; set; }

        public string? Isbn { get; set; }

        public DateTime? PublicationDate { get; set; }

        public IList<int>? AuthorsIds { get; set; }
    }
}