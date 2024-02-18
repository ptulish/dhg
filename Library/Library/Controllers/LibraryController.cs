using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Library.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LibraryController : ControllerBase
    {
        private readonly LibraryContext _db;

        public LibraryController(LibraryContext context)
        {
            _db = context;
        }


        //localhost:44319/api/library

        [HttpPost] //create 
        public IActionResult CreateBook(Book book)
        {
            _db.Books.Add(book);
            _db.SaveChanges();

            return CreatedAtAction("GetBook", new { id = book.Id}, book);
        }
        [HttpGet]
        public IActionResult GetBook(int id)
        {
            //Book bookFromDb = _db.Books.Find(id);
            Book bookFromDb = _db.Books.SingleOrDefault(b => b.Id == id);

            if (bookFromDb == null)
            {
                return NotFound();
            }

            return Ok(bookFromDb);
        }

        [HttpPut]
        public IActionResult UpdateBook(Book book)
        {
            Book bookFromDb = _db.Books.SingleOrDefault(b => b.Id == book.Id);

            if (bookFromDb == null)
            {
                return NotFound();
            }

            bookFromDb.Title = book.Title;
            bookFromDb.PageCount = book.PageCount;

            _db.SaveChanges();

            return Ok("Book updated succuesfull");

        }

        [HttpDelete]
        public IActionResult DeleteBook(int id)
        {
            Book bookFromDb = _db.Books.SingleOrDefault(b => b.Id == id);

            if (bookFromDb == null)
            {
                return NotFound();
            }

            _db.Remove(bookFromDb);
            _db.SaveChanges();

            return Ok("deleted book");
        }
        
        [HttpGet]
        [Route("GetBooks")]
        public IActionResult GetBooks()
        {
            var allBooks = _db.Books.ToList();

            if (allBooks.Count == 0)
            {
                return Ok("No books in database");
            }

            return Ok(allBooks);
        }
    }
}
