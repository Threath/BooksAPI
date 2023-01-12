using BooksAPI.Models;

namespace BooksAPI.DTO
{
    public class CreateAuthorDTO
    {

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public DateTime BirthDate { get; set; }

        public bool Gender { get; set; }

        public IList<CreateBookAuthorDTO> BookAuthors { get; set; }
    }
}
