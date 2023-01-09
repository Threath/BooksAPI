using System.ComponentModel.DataAnnotations;

namespace BooksAPI.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Rating { get; set; }
        public string ISBN { get; set; }
        public DateTime PublicationDate { get; set; }

        public virtual IList<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>() { };
    }
}
