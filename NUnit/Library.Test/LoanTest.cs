using NUnit.Framework;
using LibraryAPI.Controllers;
using LibraryAPI.Models;
using LibraryAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryAPI.Services;
using AutoFixture;

namespace LibraryAPI.Tests.Controllers
{
    [TestFixture]
    public class LoanControllerTests
    {
        private LoanController _loanController;
        private Mock<IBookService> _mockBookService;
        private Mock<ILoanService> _mockLoanService;
        private Mock<IMemberService> _mockMemberService;
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _mockBookService = new Mock<IBookService>();
            _mockLoanService = new Mock<ILoanService>();
            _mockMemberService = new Mock<IMemberService>();
            _loanController = new LoanController(_mockLoanService.Object, _mockMemberService.Object, _mockBookService.Object);
        }

        /////////////////////////////////GET ALL LOANS///////////////////////////////////////////////

        [Test]
        public async Task GetAllLoans_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var loans = _fixture.Build<Loan>()
                                    .With(m => m.LoanDate, DateOnly.MinValue)
                                    .CreateMany(3); // Generišemo nekoliko pozajmica

            _mockLoanService.Setup(service => service.Get()).ReturnsAsync(loans);

            // Act
            var result = await _loanController.Get() as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(loans));
        }


        [Test]
        public async Task GetAllLoans_NoLoans_ReturnsNotFound()
        {
            // Arrange
            IEnumerable<Loan> loans = null;

            _mockLoanService.Setup(service => service.Get()).ReturnsAsync(loans);

            // Act
            var result = await _loanController.Get() as NotFoundResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(404));
        }


        [Test]
        public async Task GetAllLoans_ServiceError_ReturnsInternalServerError()
        {
            // Arrange
            _mockLoanService.Setup(service => service.Get()).ThrowsAsync(new Exception("Simulated exception"));

            // Act
            var result = await _loanController.Get() as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(500));
            Assert.That(result.Value, Is.EqualTo("Internal Server Error"));
        }


        /////////////////////////////////ADD LOAN///////////////////////////////////////////////

        [Test]
        public async Task AddLoan_ValidLoan_ReturnsOkResult()
        {
            // Arrange
            var existingMember = _fixture.Build<Member>()
                                     .With(m => m.MembershipExpiration, DateOnly.MinValue)
                                     .Create();
            var existingBook = _fixture.Create<Book>();
            var newLoan = _fixture.Build<Loan>()
                                     .With(m => m.LoanDate, DateOnly.MinValue)
                                     .With(m => m.MemberId, existingMember.Id)
                                     .With(m => m.BookId, existingBook.Id)
                                     .Create();

            _mockMemberService.Setup(service => service.Get(newLoan.MemberId)).ReturnsAsync(existingMember);
            _mockBookService.Setup(service => service.Get(newLoan.BookId)).ReturnsAsync(existingBook);
            _mockLoanService.Setup(service => service.Create(newLoan)).ReturnsAsync(newLoan);

            // Act
            var result = await _loanController.Post(newLoan) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(newLoan));
        }

        [Test]
        public async Task AddLoan_InvalidMember_ReturnsBadRequest()
        {
            // Arrange
            var nonExistingMemberId = 999; // Ovaj ID ne postoji u bazi podataka
            var existingBook = _fixture.Create<Book>();
            var newLoan = _fixture.Build<Loan>()
                                    .With(m => m.LoanDate, DateOnly.MinValue)
                                    .With(m => m.MemberId, nonExistingMemberId)
                                    .With(m => m.BookId, existingBook.Id)
                                    .Create();

            _mockMemberService.Setup(service => service.Get(nonExistingMemberId)).ReturnsAsync((Member)null);
            _mockBookService.Setup(service => service.Get(existingBook.Id)).ReturnsAsync(existingBook);

            // Act
            var result = await _loanController.Post(newLoan) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task AddLoan_InvalidBook_ReturnsBadRequest()
        {
            // Arrange
            var existingMember = _fixture.Build<Member>()
                                     .With(m => m.MembershipExpiration, DateOnly.MinValue)
                                     .Create();
            var nonExistingBookId = 999; // Ovaj ID ne postoji u bazi podataka
            var newLoan = _fixture.Build<Loan>()
                                      .With(m => m.LoanDate, DateOnly.MinValue)
                                      .With(m => m.MemberId, existingMember.Id)
                                      .With(m => m.BookId, nonExistingBookId)
                                      .Create();

            _mockMemberService.Setup(service => service.Get(existingMember.Id)).ReturnsAsync(existingMember);
            _mockBookService.Setup(service => service.Get(nonExistingBookId)).ReturnsAsync((Book)null);

            // Act
            var result = await _loanController.Post(newLoan) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(404));
        }

        /////////////////////////////////GET LOAN///////////////////////////////////////////////

        [Test]
        public async Task GetLoan_ExistingLoanId_ReturnsOkResult()
        {
            // Arrange
            var existingLoan = _fixture.Build<Loan>()
                                      .With(m => m.LoanDate, DateOnly.MinValue)
                                      .Create();
            _mockLoanService.Setup(service => service.Get(existingLoan.Id)).ReturnsAsync(existingLoan);

            // Act
            var result = await _loanController.Get(existingLoan.Id) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(existingLoan));
        }

        [Test]
        public async Task GetLoan_NonExistingLoanId_ReturnsNotFound()
        {
            // Arrange
            var nonExistingLoanId = 999; // Ovaj ID ne postoji u bazi podataka
            _mockLoanService.Setup(service => service.Get(nonExistingLoanId)).ReturnsAsync((Loan)null);

            // Act
            var result = await _loanController.Get(nonExistingLoanId) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task GetLoan_ServiceError_ReturnsInternalServerError()
        {
            // Arrange
            var existingLoanId = 1; // Pretpostavimo da ovo postoji u bazi podataka
            _mockLoanService.Setup(service => service.Get(existingLoanId)).ThrowsAsync(new Exception("Simulated exception"));

            // Act
            var result = await _loanController.Get(existingLoanId) as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(500));
            Assert.That(result.Value, Is.EqualTo("Internal Server Error"));
        }

        /////////////////////////////////DELETE LOAN///////////////////////////////////////////////

        [Test]
        public async Task DeleteLoan_ExistingLoanId_ReturnsOkResult()
        {
            // Arrange
            var existingLoan = _fixture.Build<Loan>()
                                      .With(m => m.LoanDate, DateOnly.MinValue)
                                      .Create();
            _mockLoanService.Setup(service => service.Get(existingLoan.Id)).ReturnsAsync(existingLoan);

            // Act
            var result = await _loanController.Delete(existingLoan.Id) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(existingLoan));
        }

        [Test]
        public async Task DeleteLoan_NonExistingLoanId_ReturnsNotFound()
        {
            // Arrange
            var nonExistingLoanId = 999; // Ovaj ID ne postoji u bazi podataka
            _mockLoanService.Setup(service => service.Get(nonExistingLoanId)).ReturnsAsync((Loan)null);

            // Act
            var result = await _loanController.Delete(nonExistingLoanId) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task DeleteLoan_ServiceError_ReturnsInternalServerError()
        {
            // Arrange
            var existingLoanId = 1; // Pretpostavimo da ovo postoji u bazi podataka
            _mockLoanService.Setup(service => service.Get(existingLoanId)).ThrowsAsync(new Exception("Simulated exception"));

            // Act
            var result = await _loanController.Delete(existingLoanId) as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(500));
            Assert.That(result.Value, Is.EqualTo("Internal Server Error"));
        }
        [Test]
        public async Task Delete_InvalidData_ReturnsBadRequest()
        {
            // Arrange
            var existingLoan = _fixture.Build<Loan>()
                                      .With(m => m.LoanDate, DateOnly.MinValue)
                                      .With(m=>m.Returned,false)
                                      .Create();
            _mockLoanService.Setup(service => service.Get(existingLoan.Id)).ReturnsAsync(existingLoan);
           
            // Ne možemo obrisati pozajmicu ako ima knjiga nije vracena

            // Act
            var result = await _loanController.Delete(existingLoan.Id) as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(400));
            _mockMemberService.Verify(service => service.Delete(existingLoan.Id), Times.Never);
        }
        /////////////////////////////////EDIT LOAN///////////////////////////////////////////////

        [Test]
        public async Task UpdateLoan_ExistingLoanId_ReturnsOkResult()
        {
            // Arrange
            var existingMember = _fixture.Build<Member>()
                                     .With(m => m.MembershipExpiration, DateOnly.MinValue)
                                     .Create();
            var existingBook = _fixture.Create<Book>();
            var existingLoan = _fixture.Build<Loan>()
                                     .With(m => m.MemberId, existingMember.Id)
                                     .With(m => m.BookId, existingBook.Id)
                                     .With(m => m.LoanDate, DateOnly.MinValue)
                                     .Create();
            _mockBookService.Setup(service => service.Get(existingLoan.BookId)).ReturnsAsync(existingBook);
            _mockMemberService.Setup(service => service.Get(existingLoan.MemberId)).ReturnsAsync(existingMember);
            _mockLoanService.Setup(service => service.Get(existingLoan.Id)).ReturnsAsync(existingLoan);

            var updatedLoan = _fixture.Build<Loan>()
                                   .With(m => m.Id, existingLoan.Id)
                                   .With(m => m.BookId, existingLoan.BookId)
                                   .With(m => m.LoanDate, existingLoan.LoanDate)
                                   .With(m => m.MemberId, existingLoan.MemberId)
                                   .With(m => m.Returned, true)
                                   .Create();
            // Act
            var result = await _loanController.Update(existingLoan.Id) as OkObjectResult;//Edit menja samo Returned

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(((Loan)result.Value).Id, Is.EqualTo(existingLoan.Id));
            Assert.That(((Loan)result.Value).BookId, Is.EqualTo(existingLoan.BookId));
            Assert.That(((Loan)result.Value).LoanDate, Is.EqualTo(existingLoan.LoanDate));
            Assert.That(((Loan)result.Value).MemberId, Is.EqualTo(existingLoan.MemberId));
            Assert.That(((Loan)result.Value).Returned, Is.EqualTo(existingLoan.Returned));
        }

        [Test]
        public async Task UpdateLoan_NonExistingLoanId_ReturnsNotFound()
        {
            // Arrange
            var nonExistingLoanId = 999; // Ovaj ID ne postoji u bazi podataka

            _mockLoanService.Setup(service => service.Get(nonExistingLoanId)).ReturnsAsync((Loan)null);

            // Act
            var result = await _loanController.Update(nonExistingLoanId) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task UpdateLoan_ServiceError_ReturnsInternalServerError()
        {
            // Arrange
            var existingLoanId = 1; // Pretpostavimo da ovo postoji u bazi podataka

            _mockLoanService.Setup(service => service.Get(existingLoanId)).ThrowsAsync(new Exception("Simulated exception"));

            // Act
            var result = await _loanController.Update(existingLoanId) as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(500));
            Assert.That(result.Value, Is.EqualTo("Internal Server Error"));
        }

        /////////////////////////////////GET LOANS BY MEMBER ID///////////////////////////////////////////////

        [Test]
        public async Task GetForMember_ExistingMemberId_ReturnsOkResult()
        {
            // Arrange
            var existingMember = _fixture.Build<Member>()
                                     .With(m => m.MembershipExpiration, DateOnly.MinValue)
                                     .Create(); // Postoji u bazi podataka
            var existingLoans = _fixture.Build<Loan>()
                                     .With(m => m.MemberId, existingMember.Id)
                                     .With(m => m.LoanDate, DateOnly.MinValue)
                                     .CreateMany(3); ; // Generišemo nekoliko pozajmica
            var listOfLoans = new List<Loan>();
            foreach (var loan in existingLoans)
            {
                listOfLoans.Add(loan);
            }
            _mockMemberService.Setup(service => service.Get(existingMember.Id)).ReturnsAsync(existingMember);
            _mockLoanService.Setup(service => service.GetForMember(existingMember.Id)).ReturnsAsync(listOfLoans);

            // Act
            var result = await _loanController.GetForMember(existingMember.Id) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(listOfLoans));
        }

        [Test]
        public async Task GetForMember_NonExistingMemberId_ReturnsNotFound()
        {
            // Arrange
            var nonExistingMemberId = 999; // Ovaj član ne postoji u bazi podataka

            _mockMemberService.Setup(service => service.Get(nonExistingMemberId)).ReturnsAsync((Member)null); ;

            // Act
            var result = await _loanController.GetForMember(nonExistingMemberId) as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(400));
        }
        [Test]
        public async Task GetForMember_ExistingMemberIdLoansNULL_ReturnsNotFound()
        {
            // Arrange
            var existingMember = _fixture.Build<Member>()
                                     .With(m => m.MembershipExpiration, DateOnly.MinValue)
                                     .Create(); // Postoji u bazi podataka

            _mockMemberService.Setup(service => service.Get(existingMember.Id)).ReturnsAsync(existingMember); ;
            _mockLoanService.Setup(service => service.GetForMember(existingMember.Id)).ReturnsAsync((List<Loan>)null);
            // Act
            var result = await _loanController.GetForMember(existingMember.Id) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task GetForMember_ServiceError_ReturnsInternalServerError()
        {
            // Arrange
            var existingMember = _fixture.Build<Member>()
                                     .With(m => m.MembershipExpiration, DateOnly.MinValue)
                                     .Create(); // Postoji u bazi podataka

            _mockMemberService.Setup(service => service.Get(existingMember.Id)).ReturnsAsync(existingMember);
            _mockLoanService.Setup(service => service.GetForMember(existingMember.Id)).ThrowsAsync(new Exception("Simulated exception"));

            // Act
            var result = await _loanController.GetForMember(existingMember.Id) as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(500));
            Assert.That(result.Value, Is.EqualTo("Internal Server Error"));
        }

        /////////////////////////////////GET LOAN BY BOOK ID///////////////////////////////////////////////

        [Test]
        public async Task GetForBook_ExistingBookId_ReturnsOkResult()
        {
            // Arrange
            var existingBook = _fixture.Create<Book>();
            var existingLoans = _fixture.Build<Loan>()
                                     .With(m => m.BookId, existingBook.Id)
                                     .With(m => m.LoanDate, DateOnly.MinValue)
                                     .CreateMany(3); ; // Generišemo nekoliko pozajmica
            var listOfLoans = new List<Loan>();
            foreach (var loan in existingLoans)
            {
                listOfLoans.Add(loan);
            }
            _mockBookService.Setup(service => service.Get(existingBook.Id)).ReturnsAsync(existingBook);
            _mockLoanService.Setup(service => service.GetForBook(existingBook.Id)).ReturnsAsync(listOfLoans);

            // Act
            var result = await _loanController.GetForBook(existingBook.Id) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(listOfLoans));
        }

        [Test]
        public async Task GetForBook_NonExistingBookId_ReturnsNotFound()
        {
            // Arrange
            var nonExistingBookId = 999; // Ovaj član ne postoji u bazi podataka

            _mockBookService.Setup(service => service.Get(nonExistingBookId)).ReturnsAsync((Book)null); ;

            // Act
            var result = await _loanController.GetForBook(nonExistingBookId) as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(400));
        }

        [Test]
        public async Task GetForBook_ServiceError_ReturnsInternalServerError()
        {
            // Arrange
            var existingBook = _fixture.Create<Book>();

            _mockBookService.Setup(service => service.Get(existingBook.Id)).ReturnsAsync(existingBook);
            _mockLoanService.Setup(service => service.GetForBook(existingBook.Id)).ThrowsAsync(new Exception("Simulated exception"));

            // Act
            var result = await _loanController.GetForBook(existingBook.Id) as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(500));
            Assert.That(result.Value, Is.EqualTo("Internal Server Error"));
        }
        [Test]
        public async Task GetForBook_ExistingBookIdLoansNULL_ReturnsNotFound()
        {
            // Arrange
            var existingBook = _fixture.Create<Book>();// Postoji u bazi podataka

            _mockBookService.Setup(service => service.Get(existingBook.Id)).ReturnsAsync(existingBook);
            _mockLoanService.Setup(service => service.GetForBook(existingBook.Id)).ReturnsAsync((List<Loan>)null);
            // Act
            var result = await _loanController.GetForBook(existingBook.Id) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(404));
        }
    }
}
