using LibraryAPI.Models;
using LibraryAPI.Services;
using LibraryAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoanController : ControllerBase
    {
        private readonly ILoanService _loanService;
        private readonly IMemberService _memberService;
        private readonly IBookService _bookService;

        public LoanController(ILoanService loanService, IMemberService memberService, IBookService bookService)
        {
            _loanService = loanService;
            _memberService = memberService;
            _bookService = bookService;
        }

        [HttpGet]
        [Route("GetAllLoans")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var loans = await _loanService.Get();
                if (loans == null)
                {
                    return NotFound();
                }
                return Ok(loans);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet]
        [Route("GetLoan/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var loan = await _loanService.Get(id);

                if (loan is null)
                {
                    return NotFound();
                }

                return Ok(loan);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost]
        [Route("AddLoan")]
        public async Task<IActionResult> Post(Loan newLoan)
        {
            var member = await _memberService.Get(newLoan.MemberId);
            var book = await _bookService.Get(newLoan.BookId);
            if (member is not null && book is not null)
            {
                if (book.Available > 1)
                {
                    var updatedBook = new Book();
                    updatedBook.Id = book.Id;
                    updatedBook.Title = book.Title;
                    updatedBook.Description = book.Description;
                    updatedBook.Author = book.Author;
                    updatedBook.NumberOfCopies = book.NumberOfCopies;
                    updatedBook.Available = book.Available - 1;
                    newLoan.LoanDate = DateOnly.FromDateTime(DateTime.Now);
                    newLoan.Returned = false;

                    await _loanService.Create(newLoan);
                    await _bookService.Update(book.Id, updatedBook);
                }
                else
                {
                    return BadRequest(new
                    {
                        ErrorMessage = "This book is no longer available."
                    });
                }
            }
            else
            {
                return NotFound();
            }

            return Ok(newLoan);
        }

        [HttpPut]
        [Route("EditLoan/{id}")]
        public async Task<IActionResult> Update(int id)
        {
            try
            {
                var loan = await _loanService.Get(id);
                if (loan is null)
                {
                    return NotFound();
                }
                var updatedLoan = new Loan();
                updatedLoan.Id = id;
                updatedLoan.MemberId = loan.MemberId;
                updatedLoan.BookId = loan.BookId;
                updatedLoan.LoanDate = loan.LoanDate;
                updatedLoan.Returned = true;

                var book = await _bookService.Get(loan.BookId);
                if (book is not null)
                {
                    var updatedBook = new Book();
                    updatedBook.Id = book.Id;
                    updatedBook.Title = book.Title;
                    updatedBook.Description = book.Description;
                    updatedBook.Author = book.Author;
                    updatedBook.NumberOfCopies = book.NumberOfCopies;
                    updatedBook.Available = book.Available + 1;

                    await _bookService.Update(book.Id, updatedBook);
                }
                await _loanService.Update(id, updatedLoan);

                return Ok(updatedLoan);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpDelete]
        [Route("DeleteLoan/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var loan = await _loanService.Get(id);
                if (loan is null)
                {
                    return NotFound();
                }
                if (!loan.Returned)
                {
                    return BadRequest(new
                    {
                        ErrorMessage = "Can't delete loan,book isn't returnd"
                    });
                }
                await _loanService.Delete(id);

                return Ok(loan);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet]
        [Route("GetLoansByMemberId/{id}")]
        public async Task<IActionResult> GetForMember(int id)
        {

            try
            {
                var member = await _memberService.Get(id);
                if (member == null)
                {
                    return BadRequest("Invalid member");
                }
                var loans = await _loanService.GetForMember(id);

                if (loans is null)
                {
                    return NotFound();
                }

                return Ok(loans);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet]
        [Route("GetLoansByBookId/{id}")]
        public async Task<IActionResult> GetForBook(int id)
        {
            try
            {
                var book = await _bookService.Get(id);
                if (book == null)
                {
                    return BadRequest("Invalid book");
                }
                var loans = await _loanService.GetForBook(id);

                if (loans is null)
                {
                    return NotFound();
                }

                return Ok(loans);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
