using BooksAPI.DTO;
using BooksAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BooksAPI.Controllers
{
    [Route("api/Books")]
    [ApiController]
    public class AuthorsController : Controller
    {
        private readonly BooksDbContext _context;

        public AuthorsController(BooksDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetAuthors")]
        public async Task<ActionResult<IEnumerable<Author>>> GetAuthors()
        {
            if (_context.Author == null)
            {
                return NotFound();
            }

            try
            {
                await _context.Author.ToListAsync();
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to fetch authors");
            }
            var authors = _context.Author.ToList();
            return Ok(authors);
        }

        [HttpGet("GetAuthor/{FirstName}&&{LastName}")]
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

        // POST: api/Author
        [HttpPost("PostAuthor")]
        public async Task<ActionResult<Author>> CreateAuthor(AuthorDTO request)
        {
            //validate if none of the values are null, didnt know how to exclude one property from a condition
            if (request.BirthDate==null||request.FirstName==null||request.LastName==null||request.Gender==null)
            {
                return BadRequest("Thats a bad request");
            }
            if(_context.Author.Any(a=> request.FirstName == a.FirstName&&request.LastName==a.LastName))
            {
                return BadRequest("Taki autor juz istnieje");
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
                return StatusCode(500, "Failed to create an author");
            }
            //if there are any books added by query, add the FK relationship, AND VALIDATE THEIR IDs first
            if(request.BooksIds != null)
            {
                if (request.BooksIds.Count != 0&&!request.BooksIds.All(ba => _context.Author.Any(a => a.Id == ba)))
                {

                    for (int i = 0; i < request.BooksIds.Count(); ++i)
                    {
                        //retrieve the BookId from the db
                        int BookId = request.BooksIds[i];
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
                            return StatusCode(500, "Failed to create the BookAuthor object");
                        }
                    }
                }
            }

            return StatusCode(201);
        }
    }
}