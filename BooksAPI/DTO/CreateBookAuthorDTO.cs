using BooksAPI.Models;
using System.Runtime.Serialization;

namespace BooksAPI.DTO
{
    public class CreateBookAuthorDTO
    {
        public virtual BookDTO? Book { get; set; }

        public virtual AuthorDTO? Author { get; set; }
    }
}
