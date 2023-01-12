using BooksAPI.Models;
using System.Runtime.Serialization;

namespace BooksAPI.DTO
{
    public class CreateBookAuthorDTO
    {
        public virtual CreateBookDTO Book { get; set; }

        public virtual CreateAuthorDTO Author { get; set; }
    }
}
