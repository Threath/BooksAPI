using Microsoft.EntityFrameworkCore;
using System.Numerics;
using System.Runtime.Serialization;

namespace BooksAPI.Models
{
    public class BookAuthor
    {
        public int BookId { get; set; }

        public virtual Book Book { get; set; }

        public int AuthorId { get; set; }

        public virtual Author Author { get; set; }
    }
}
