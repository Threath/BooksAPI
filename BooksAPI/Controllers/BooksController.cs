using BooksAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.Entity;

namespace BooksAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private BooksContext _booksContext;

        public BooksController(BooksContext booksContext)
        {
            _booksContext = booksContext;
        }

        [HttpGet("GetBooks")]
        public IActionResult GetBooks()
        {
            try
            {
                var books = _booksContext.Book.ToList();
                if (books.Count == 0)
                {
                    return StatusCode(404, "There are no books");
                }
                return Ok(books);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpGet("GetAuthors")]
        public IActionResult GetAuthors()
        {
            try
            {
                var Authors = _booksContext.Author.ToList();
                if (Authors.Count == 0)
                {
                    return StatusCode(404, "AThere are no authors");
                }
                return Ok(Authors);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpPost("CreateBook")]
        public IActionResult CreateBook([FromBody] Book req)
        {
            Book book = new Book()
            {
                Title = req.Title,
                Description = req.Description,
                Rating = req.Rating,
                ISBN = req.ISBN,
                PublicationDate = req.PublicationDate
            };
            try
            {
                _booksContext.Book.Add(book);
                _booksContext.SaveChanges();
                var books = _booksContext.Book.ToList();
                return Ok(books);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpPost("CreateAuthor")]
        public IActionResult CreateAuthor([FromBody] Author req)
        {
            Author author = new Author();
            author.Id = _booksContext.Author.ToList().Count + 1;
            author.FirstName = req.FirstName;
            author.LastName = req.LastName;
            author.BirthDate = req.BirthDate;
            author.Gender = req.Gender;

            try
            {
                _booksContext.Author.Add(author);
                _booksContext.SaveChanges();
                var Authors = _booksContext.Author.ToList();
                return Ok(Authors);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpPut("UpdateBook")]
        public IActionResult UpdateBook([FromBody] Book req)
        {
            try
            {
                var book = _booksContext.Book.FirstOrDefault(x => x.Id == req.Id);
                if (book == null)
                {
                    return StatusCode(404, "book not found");
                }
                book.Title = req.Title;
                book.Description = req.Description;
                book.Rating = req.Rating;
                book.ISBN = req.ISBN;
                book.PublicationDate = req.PublicationDate;
                _booksContext.SaveChanges();
                var books = _booksContext.Book.ToList();
                return Ok(books);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpDelete("DeleteBook/{Id}")]
        public IActionResult DeleteBook([FromRoute] int Id)
        {
            try
            {
                var book = _booksContext.Book.FirstOrDefault(x => x.Id == Id);
                if (book == null)
                {
                    return StatusCode(404, "book not found");
                }
                _booksContext.Entry(book).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
                _booksContext.SaveChanges();

                var books = _booksContext.Book.ToList();
                return Ok(books);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpGet("GetAuthor/{FirstName}&&{LastName}")]
        public IActionResult GetAuthorByName([FromRoute] string FirstName, string LastName)
        {
            try
            {
                var author = _booksContext.Author.FirstOrDefault(
                    x => (x.FirstName == FirstName && x.LastName == LastName)
                );
                if (author == null)
                {
                    return StatusCode(404, "No author is named: " + FirstName + " " + LastName);
                }
                return Ok(author);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpGet("GetBook/{Title}")]
        public IActionResult GetBookByTitle([FromRoute] string Title)
        {
            try
            {
                var book = _booksContext.Book.FirstOrDefault(x => x.Title == Title);
                if (book == null)
                {
                    return StatusCode(404, "Book not found");
                }
                return Ok(book);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }
    }
}
