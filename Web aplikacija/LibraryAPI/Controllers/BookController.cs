using LibraryAPI.Models;
using LibraryAPI.Services;
using LibraryAPI.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;
        private readonly ILoanService _loanService;
        public BookController(IBookService bookService, ILoanService loanService)
        {
            _bookService = bookService;
            _loanService = loanService;
        }

        [HttpGet]
        [Route("GetAllBooks")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var books = await _bookService.Get();
                if (books == null)
                {
                    return NotFound();
                }
                return Ok(books);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }
        [HttpGet]
        [Route("GetBook/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var book = await _bookService.Get(id);

                if (book is null)
                {
                    return NotFound();
                }

                return Ok(book);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }
        [HttpGet]
        [Route("GetBookTitle/{id}")]
        public async Task<IActionResult> GetTitle(int id)
        {
            try
            {
                var book = await _bookService.Get(id);

                if (book is null)
                {
                    return NotFound();
                }
                var name = book.Title + " " + book.Author;

                return Ok(new { title = book.Title, author = book.Author });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost]
        [Route("AddBook")]
        public async Task<IActionResult> Post(Book newBook)
        {
            try
            {
                if (newBook == null || string.IsNullOrWhiteSpace(newBook.Title) || string.IsNullOrWhiteSpace(newBook.Author))
                {
                    return BadRequest(new
                    {
                        ErrorMessage = "Title and Author are required"
                    });
                }

                if (newBook.NumberOfCopies == 0)
                {
                    return BadRequest(new
                    {
                        ErrorMessage = "Number of copies can't be 0"
                    });
                }
                newBook.Available = newBook.NumberOfCopies;

                await _bookService.Create(newBook);

                return Ok(newBook);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }


        [HttpPut]
        [Route("EditBook/{id}")]
        public async Task<IActionResult> Update(int id, Book updatedBook)
        {
            var book = await _bookService.Get(id);
            if (book is null)
            {
                return NotFound();
            }

            if (updatedBook == null || string.IsNullOrWhiteSpace(updatedBook.Title) || string.IsNullOrWhiteSpace(updatedBook.Author))
            {
                return BadRequest(new
                {
                    ErrorMessage = "Title and Author are required"
                });
            }


            if (updatedBook.NumberOfCopies != book.NumberOfCopies)
            {
                if (updatedBook.NumberOfCopies == 0)
                {
                    return BadRequest(new
                    {
                        ErrorMessage = "Number of copies can't be 0"
                    });
                }
                if (updatedBook.NumberOfCopies >= book.NumberOfCopies - book.Available)
                {
                    updatedBook.Available = book.Available + (updatedBook.NumberOfCopies - book.NumberOfCopies);
                }
                else
                {
                    var remainingBooks = book.NumberOfCopies - book.Available;
                    return BadRequest(new
                    {
                        ErrorMessage = "Number of copies must be greater than:" + remainingBooks,
                        RemainingBook = remainingBooks
                    });
                }
            }

            await _bookService.Update(id, updatedBook);

            return Ok(updatedBook);
        }

        [HttpDelete]
        [Route("DeleteBook/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _bookService.Get(id);
            if (book is null)
            {
                return NotFound();
            }
            var loans = await _loanService.GetForBook(id);
            if (loans != null)
            {
                foreach (var loan in loans)
                {
                    if (loan.Returned == false)
                    {
                        return BadRequest(
                            new
                            {
                                ErrorMessage = "Can't delete book,there are loans for this book that are not returned"
                            });
                    }
                }
            }
            await _bookService.Delete(id);

            return Ok(book);
        }
    }
}
