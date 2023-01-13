namespace BooksAPI.DTO
{
    public class BookAuthorDTO
    {
        public virtual BookDTO? Book { get; set; }

        public virtual AuthorDTO? Author { get; set; }
    }
}