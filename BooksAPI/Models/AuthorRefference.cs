namespace BooksAPI.Models
{
    public class AuthorRefference
    {
        public int Id { get; set; } = 0;
        public string FirstName { get; set; } = 0;

        public string LastName { get; set; } = 0;

        public DateTime BirthDate { get; set; } = 0;
        public bool Gender { get; set; } = 0;

        public IList<BookAuthorRefference> BookAuthors { get; set; } =
            new List<BookAuthorRefference>();
    }
}
