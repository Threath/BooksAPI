using BooksAPI.Models;
using Microsoft.Build.Framework;

namespace BooksAPI.DTO
{
    public class AuthorDTO
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }
        
        public DateTime? BirthDate { get; set; }

        public bool? Gender { get; set; }

        public IList<CreateBookAuthorDTO>? BookAuthors { get; set; }
    }
}
