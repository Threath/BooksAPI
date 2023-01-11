using System;
using System.Collections.Generic;

namespace BooksAPI.Models;

public class Author
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateTime BirthDate { get; set; }

    public bool Gender { get; set; }

    public IList<BookAuthor> BookAuthors { get; set; }
}
