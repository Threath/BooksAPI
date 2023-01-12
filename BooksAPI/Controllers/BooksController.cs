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
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Data.Entity;
using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations;

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
        public async Task<ActionResult<Author>> CreateAuthor(AuthorDTO request)
        {
            //validate if none of the values are null
            if (request.GetType().GetProperties().Any(prop => prop.GetValue(request) == null))
            {
                return BadRequest("Thas a bad request");
            }

            //create a new Author instance
            Author author = new Author()
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                BirthDate = (DateTime)request.BirthDate,
                Gender = (bool)request.Gender
            };

            _context.Author.Add(author);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "Failed to create a book");
            }
            //if there are any books written by the author, add the FK relationship
            if(request.BookAuthors.Count!=0)
            {
                for (int i = 0; i < request.BookAuthors.Count(); ++i)
                {
                    //retrieve the BookId from the db
                    int BookId = _context.Book.FirstOrDefault(b => request.BookAuthors[i].Book.Isbn == b.Isbn).Id;
                    //retrieve AuthorId from the db
                    int AuthorId = _context.Author.FirstOrDefault(a => (request.FirstName == a.FirstName && request.LastName == a.LastName)).Id;

                    BookAuthor bookAuthor = new BookAuthor();
                    bookAuthor.AuthorId = AuthorId;
                    bookAuthor.BookId = BookId;
                    _context.BookAuthor.Add(bookAuthor);

                    try
                    {
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateException)
                    {
                        return StatusCode(500, "Failed to create a book");
                    }

                }
            }
          
            return CreatedAtAction("GetBooks", _context.Author.FirstOrDefault());
        }

        // POST: api/Books
        //it works but it's ugly
        [HttpPost("PostBook")]
        public async Task<ActionResult<Book>> CreateBook(BookDTO request)
        {
            //check if any book property is a null
            if(request.GetType().GetProperties().Any(prop=>prop.GetValue(request)==null))
            {
                return BadRequest("Thas a bad request");
            }

            //validate if the authors were input
            if (request.BookAuthors.All(ba=>(ba.Author.FirstName==null&&ba.Author.LastName == null)))
            {
                return BadRequest("Dodaj authora");
            }

            //validate if the authors exist in the db
            if(!request.BookAuthors.All(ba => _context.Author.Any(a=>(a.FirstName==ba.Author.FirstName&&a.LastName==ba.Author.LastName))))
            {
                return BadRequest("Ci authorzy nie istnieja");
            }

            //validate if the ISBN exists
            if (_context.Book.Any(b => b.Isbn == request.Isbn))
            {
                return BadRequest("Ksiazka o tym numerze ISBN juz istnieje");
            }

            //create new book instance
            Book book = new Book()
            {
                Description = request.Description,
                Title = request.Title,
                PublicationDate = (DateTime)request.PublicationDate,
                Isbn = request.Isbn,
                Rating = (int)request.Rating
            };
            _context.Book.Add(book);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "Failed to create a book");
            }

            //for each author create new FK record
            for (int i = 0;i < request.BookAuthors.Count() ; ++i)
            {

                //Find Id of the newly added book from db
                int BookId = _context.Book.FirstOrDefault(b => b.Isbn == request.Isbn).Id;
                //find the Id of the author from request
                int AuthorId = _context.Author.FirstOrDefault(a => (request.BookAuthors[i].Author.FirstName == a.FirstName) && (request.BookAuthors[i].Author.LastName == a.LastName)).Id;

                BookAuthor bookAuthor = new BookAuthor();
                bookAuthor.AuthorId = AuthorId;
                bookAuthor.BookId = BookId;
                _context.BookAuthor.Add(bookAuthor);

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    return StatusCode(500, "Failed to create a book");
                }
            }
            return CreatedAtAction("GetBooks", _context.Book.FirstOrDefault());
        }


        // PUT: api/Books/5
        [HttpPut("PutBook/{id}")]
        public async Task<IActionResult> UpdateBook(int id, BookDTO request)
        {
            //check if any book property is a null
            if (request.GetType().GetProperties().Any(prop => prop.GetValue(request) == null))
            {
                return BadRequest("Thas a bad request");
            }
            //validate if the ISBN exists
            if (_context.Book.Any(b => b.Isbn != request.Isbn))
            {
                return BadRequest("Ksiazka o tym numerze ISBN juz istnieje");
            }

            _context.Entry(request).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

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

            int quantity = _context.BookAuthor.Where(ba=>ba.BookId == id).Count();
            //delete all FK links
            for(int i = 0; i < quantity;++i)
            {
                //find the Id of the FK
                int BookId = _context.BookAuthor.FirstOrDefault(ba => ba.BookId == id).BookId;
                //Find first FK with the Id
                var bookAuthor =  _context.BookAuthor.FirstOrDefault(ba=> ba.BookId == BookId);
                if (bookAuthor == null)
                {
                    return NotFound();
                }
                _context.BookAuthor.Remove(bookAuthor);
                
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {

                    return StatusCode(500, "failed to delete the book");
                }
            }    
            //delete the object
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
