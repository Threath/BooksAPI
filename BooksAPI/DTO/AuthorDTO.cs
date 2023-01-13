namespace BooksAPI.DTO
{
    public class AuthorDTO
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public DateTime? BirthDate { get; set; }

        public bool? Gender { get; set; }

        public IList<int>? BooksIds { get; set; }
    }
}