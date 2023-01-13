using BooksAPI.src.Api.DTO;
using BooksAPI.src.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace BooksAPI.src.Api.Controllers
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

        // GET: api/Books/5
        [HttpGet("GetBook/{Isbn}")]
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

        // POST: api/Books
        //it works but it's ugly
        [HttpPost("PostBook")]
        public async Task<ActionResult<Book>> CreateBook(BookDTO request)
        {
            //check if any book property is a null
            if (request.GetType().GetProperties().Any(prop => prop.GetValue(request) == null))
            {
                return BadRequest("Wypelnij wszystkie wymagane miejsca");
            }

            //validate if the authors were input
            if (request.AuthorsIds.IsNullOrEmpty())
            {
                return BadRequest("Dodaj authora");
            }

            //validate if the authors exist in the db
            if (!request.AuthorsIds.All(ba => _context.Author.Any(a => a.Id == ba)))
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
            //Find Id of the newly added book from db
            int BookId = (int)_context.Entry(book).OriginalValues["Id"];
            book = await _context.Book.FirstOrDefaultAsync(b => b.Id == BookId);
            //for each author create new FK record
            for (int i = 0; i < request.AuthorsIds.Count(); ++i)
            {
                //find the Id of the author from request
                int AuthorId = request.AuthorsIds[i];

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

            return StatusCode(201);
        }

        // PUT: api/Books/5
        [HttpPut("PutBook/{id}")]
        public async Task<IActionResult> UpdateBook(int id, BookDTO request)
        {
            //check if any book property is a null, this is super ugly, dunno how to validate every property except one
            if (request.Isbn == null || request.PublicationDate == null || request.Title == null || request.Description == null || request.Rating == null)
            {
                return BadRequest("Wypelnij wszystkie wymagane pola");
            }
            //validate if the ISBN exists
            if (_context.Book.Any(b => b.Isbn == request.Isbn))
            {
                return BadRequest("Ksiazka o tym numerze ISBN juz istnieje");
            }
            Book book = await _context.Book.FirstOrDefaultAsync(b => b.Id == id);
            book.Title = request.Title;
            book.Description = request.Description;
            book.Isbn = request.Isbn;
            book.PublicationDate = (DateTime)request.PublicationDate;
            book.Rating = (decimal)request.Rating;

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

            int quantity = _context.BookAuthor.Where(ba => ba.BookId == id).Count();
            //delete all FK links
            for (int i = 0; i < quantity; ++i)
            {
                //find the Id of the FK
                int BookId = _context.BookAuthor.FirstOrDefault(ba => ba.BookId == id).BookId;
                //Find first FK with the Id
                var bookAuthor = _context.BookAuthor.FirstOrDefault(ba => ba.BookId == BookId);
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

        private bool BookExists(int id)
        {
            return (_context.Book?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}