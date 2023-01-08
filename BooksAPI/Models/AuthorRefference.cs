namespace BooksAPI.Models
{
    public class AuthorRefference
    {
        public int Id { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime BirthDate { get; set; }
        public bool Gender { get; set; }
    }
}
