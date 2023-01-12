using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BooksAPI.Models;
using BooksAPI.DTO;
using Azure.Core;

namespace BooksAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly BooksDbContext _context;

        public BooksController(BooksDbContext context)
        {
            _context = context;
        }

        // GET: api/Author
        // Get all Author
        [HttpGet("GetAuthor")]
        public async Task<ActionResult<IEnumerable<Author>>> GetAuthor()
        {
            if (_context.Author == null)
            {
                return NotFound();
            }
            return await _context.Author.ToListAsync();
        }

        // GET: api/Books
        // Get all books
        [HttpGet("GetBooks")]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
            if (_context.Book == null)
            {
                return NotFound();
            }
            return await _context.Book.ToListAsync();
        }

        // GET: api/Author/5
        [HttpGet("GetAuthor{FirstName}&&{LastName}")]
        public async Task<ActionResult<Author>> GetAuthor(string FirstName, string LastName)
        {
            if (_context.Author == null)
            {
                return NotFound();
            }
            var author = _context.Author.FirstOrDefault(
                x => x.FirstName == FirstName && x.LastName == LastName
            );

            if (author == null)
            {
                return NotFound();
            }

            return author;
        }

        // GET: api/Books/5
        [HttpGet("GetBook{Isbn}")]
        public async Task<ActionResult<Book>> GetBook(string Isbn)
        {
            if (_context.Book == null)
            {
                return NotFound();
            }
            var book = _context.Book.FirstOrDefault(x => Isbn == x.Isbn);

            if (book == null)
            {
                return NotFound();
            }

            return book;
        }

        // POST: api/Author
        [HttpPost("PostAuthor")]
        public async Task<ActionResult<Author>> CreateAuthor(CreateAuthorDTO request)
        {
            if (_context.Author == null)
            {
                return Problem("Entity set 'BooksDbContext.Author'  is null.");
            }
            Author author = new Author()
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Gender= request.Gender,
                BirthDate= request.BirthDate
            };
            List<BookAuthor> authorList = new List<BookAuthor>(1) { };
            int i = 0;
            foreach (var bookAuthor in authorList)
            {
                Book book = new Book()
                {
                    Description = request.BookAuthors[i].Book.Description,
                    Title = request.BookAuthors[i].Book.Title,
                    PublicationDate = request.BookAuthors[i].Book.PublicationDate,
                    Isbn = request.BookAuthors[i].Book.Isbn,
                    Rating = request.BookAuthors[i].Book.Rating
                };
                Author author1 = new Author()
                {
                    FirstName = request.BookAuthors[i].Author.FirstName,
                    LastName = request.BookAuthors[i].Author.LastName,
                    BirthDate = request.BookAuthors[i].Author.BirthDate,
                    Gender = request.BookAuthors[i].Author.Gender
                };
                bookAuthor.Author = author;
                bookAuthor.Book = book;
                i++;
            }
            author.BookAuthors = authorList;

            _context.Author.Add(author);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (AuthorExists(author.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetAuthor", author);
        }

        // POST: api/Books
        //it works but it's ugly
        [HttpPost("PostBook")]
        public async Task<ActionResult<Book>> CreateBook(CreateBookDTO request)
        {
            if (_context.Book == null)
            {
                return Problem("Entity set 'BooksDbContext.Books'  is null.");
            }

            Book book = new Book() {
                Description = request.Description,
                Title = request.Title,
                PublicationDate = request.PublicationDate,
                Isbn= request.Isbn,
                Rating= request.Rating
            };
            List<BookAuthor> authorList = new List<BookAuthor>();

            for (int i = 0; i < request.BookAuthors.Count(); ++i)
            {
                Book book1 = new Book()
                {
                    Description = request.BookAuthors[i].Book.Description,
                    Title = request.BookAuthors[i].Book.Title,
                    PublicationDate = request.BookAuthors[i].Book.PublicationDate,
                    Isbn = request.BookAuthors[i].Book.Isbn,
                    Rating = request.BookAuthors[i].Book.Rating
                };
                Author author = new Author()
                {
                    FirstName = request.BookAuthors[i].Author.FirstName,
                    LastName = request.BookAuthors[i].Author.LastName,
                    BirthDate = request.BookAuthors[i].Author.BirthDate,
                    Gender = request.BookAuthors[i].Author.Gender
                };
                BookAuthor bookauthor = new BookAuthor()
                {
                    Author = author,
                    Book = book1
                };
                authorList.Add(bookauthor);
            }
            book.BookAuthors = authorList;

            //verifying if the book has authors
            //if (book.BookAuthors.All(p => p.Author == null))
            //{
            //    return StatusCode(400, "Dodaj autora");
            //}

            
            //adding the book
            _context.Book.Add(book);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "Failed to create a book");
            }

            return CreatedAtAction("GetBooks", book);
        }

        // PUT: api/Books/5
        [HttpPut("PutBook/{id}")]
        public async Task<IActionResult> UpdateBook(int id, Book book)
        {
            if (id != book.Id)
            {
                return BadRequest();
            }

            _context.Entry(book).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok();
        }

        // DELETE: api/Books/5
        [HttpDelete("DeleteBook/{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            if (_context.Book == null)
            {
                return NotFound();
            }
            var book = await _context.Book.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            _context.Book.Remove(book);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool AuthorExists(int id)
        {
            return (_context.Author?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private bool BookExists(int id)
        {
            return (_context.Book?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
