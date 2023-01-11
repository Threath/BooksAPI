﻿using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Numerics;

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