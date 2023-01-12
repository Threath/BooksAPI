using BooksAPI.Models;

namespace BooksAPI.DTO
{
    public class CreateBookDTO
    {

        public string Title { get; set; } = null!;

        public string Description { get; set; } = null!;

        public decimal Rating { get; set; }

        public string Isbn { get; set; } = null!;

        public DateTime PublicationDate { get; set; }

        public IList<CreateBookAuthorDTO> BookAuthors { get; set; }
    }
}
