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
    public class BookControllerTests
    {
        private BookController _bookController;
        private Mock<IBookService> _mockBookService;
        private Mock<ILoanService> _mockLoanService;
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _mockBookService = new Mock<IBookService>();
            _mockLoanService = new Mock<ILoanService>();
            _bookController = new BookController(_mockBookService.Object, _mockLoanService.Object);
        }

        /////////////////////////////////GET ALL BOOKS///////////////////////////////////////////////

        [Test]
        public async Task GetAllBooks_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var books = _fixture.CreateMany<Book>(3); // Generišemo nekoliko knjiga

            _mockBookService.Setup(service => service.Get()).ReturnsAsync(books);

            // Act
            var result = await _bookController.Get() as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(books));
        }


        [Test]
        public async Task GetAllBooks_NoBooks_ReturnsNotFound()
        {
            // Arrange
            IEnumerable<Book> books = null;

            _mockBookService.Setup(service => service.Get()).ReturnsAsync(books);

            // Act
            var result = await _bookController.Get() as NotFoundResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(404));
        }


        [Test]
        public async Task GetAllBooks_ServiceError_ReturnsInternalServerError()
        {
            // Arrange
            _mockBookService.Setup(service => service.Get()).ThrowsAsync(new Exception("Simulated exception"));

            // Act
            var result = await _bookController.Get() as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(500));
            Assert.That(result.Value, Is.EqualTo("Internal Server Error"));
        }


        /////////////////////////////////ADD BOOK///////////////////////////////////////////////

        [Test]
        public async Task Post_ValidBook_ReturnsOkResult()
        {
            // Arrange
            var newBook = _fixture.Create<Book>();
            //var newBook = new Book { Title = "Test Book", Author = "Test Author", NumberOfCopies = 5 };

            // Act
            var result = await _bookController.Post(newBook) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(newBook));
        }

        [Test]
        public async Task Post_InvalidBook_ReturnsBadRequest()
        {
            // Arrange
            var invalidBook = _fixture.Build<Book>().Without(m => m.Title).Create(); 

            // Act
            var result = await _bookController.Post(invalidBook) as BadRequestObjectResult;

            // Assert+
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(400));
        }
        [Test]
        public async Task Post_InvalidBook2_ReturnsBadRequest()
        {
            // Arrange
            var invalidBook = _fixture.Build<Book>().With(m => m.NumberOfCopies,0).Create();

            // Act
            var result = await _bookController.Post(invalidBook) as BadRequestObjectResult;

            // Assert+
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(400));
        }
        [Test]
        public async Task Post_EmptyBook_ReturnsBadRequest()
        {
            // Arrange 
            var newBook = new Book();

            // Act
            var result = await _bookController.Post(newBook) as BadRequestObjectResult;

            // Assert+
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(400));
        }
        [Test]
        public async Task Post_ServiceError_ReturnsInternalServerError()
        {
            // Arrange
            _mockBookService.Setup(service => service.Create(It.IsAny<Book>())).ThrowsAsync(new Exception("Simulated exception"));
            var newBook = new Book { Title = "Test Book", Author = "Test Author", NumberOfCopies = 5 };

            // Act
            var result = await _bookController.Post(newBook) as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(500));
            Assert.That(result.Value, Is.EqualTo("Internal Server Error"));
        }

        /////////////////////////////////EDIT BOOK///////////////////////////////////////////////

        [Test]
        public async Task Update_ValidBook_ReturnsOkResult()
        {
            // Arrange
            var existingBook = _fixture.Build<Book>()
                                    .With(m => m.NumberOfCopies, 20)
                                    .Create();
            var updatedBook = _fixture.Build<Book>()
                                    .With(m => m.NumberOfCopies, 20)
                                    .Create();

            _mockBookService.Setup(service => service.Get(existingBook.Id)).ReturnsAsync(existingBook);

            // Act
            var result = await _bookController.Update(existingBook.Id, updatedBook) as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(updatedBook));
            _mockBookService.Verify(service => service.Update(existingBook.Id, updatedBook), Times.Once);
        }

        [Test]
        public async Task Update_InvalidBook_ReturnsNotFound()
        {
            // Arrange
            var nonExistingBookId = 999; // ovaj ID ne postoji u bazi podataka
            var updatedBook = _fixture.Create<Book>();

            _mockBookService.Setup(service => service.Get(nonExistingBookId)).ReturnsAsync((Book)null);

            // Act
            var result = await _bookController.Update(nonExistingBookId, updatedBook) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(404));
            _mockBookService.Verify(service => service.Update(nonExistingBookId, updatedBook), Times.Never);
        }

        [Test]
        public async Task Update_InvalidBook_ReturnsBadRequest()
        {
            // Arrange
            var existingBook = _fixture.Create<Book>();
            var updatedBook = _fixture.Build<Book>().Without(m => m.Title).Create(); // Izmena bez validnih podataka

            _mockBookService.Setup(service => service.Get(existingBook.Id)).ReturnsAsync(existingBook);

            // Act
            var result = await _bookController.Update(existingBook.Id, updatedBook) as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(400));
            _mockBookService.Verify(service => service.Update(existingBook.Id, updatedBook), Times.Never);
        }
        [Test]
        public async Task Update_InvalidBook2_ReturnsBadRequest()
        {
            // Arrange
            var existingBook = _fixture.Build<Book>().With(m => m.Available,10).With(m => m.NumberOfCopies, 20).Create();
            var updatedBook = _fixture.Build<Book>().With(m => m.NumberOfCopies,3).Create(); // Izmena bez validnih podataka

            _mockBookService.Setup(service => service.Get(existingBook.Id)).ReturnsAsync(existingBook);
            var remainingBooks = existingBook.NumberOfCopies - existingBook.Available;
            // Act
            var result = await _bookController.Update(existingBook.Id, updatedBook) as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(400));
            Assert.That(result.Value.GetType().GetProperty("ErrorMessage").GetValue(result.Value, null), Is.EqualTo("Number of copies must be greater than:"+ remainingBooks));
            Assert.That(result.Value.GetType().GetProperty("RemainingBook").GetValue(result.Value, null), Is.EqualTo(remainingBooks));
            _mockBookService.Verify(service => service.Update(existingBook.Id, updatedBook), Times.Never);
        }

        /////////////////////////////////DELETE BOOK///////////////////////////////////////////////

        [Test]
        public async Task Delete_ValidId_ReturnsOkResult()
        {
            // Arrange
            var existingBook = _fixture.Create<Book>();

            _mockBookService.Setup(service => service.Get(existingBook.Id)).ReturnsAsync(existingBook);

            // Act
            var result = await _bookController.Delete(existingBook.Id) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(existingBook));
            _mockBookService.Verify(service => service.Delete(existingBook.Id), Times.Once);
        }

        [Test]
        public async Task Delete_InvalidId_ReturnsNotFound()
        {
            // Arrange
            var nonExistingBookId = 999; // Ovaj ID ne postoji u bazi podataka

            _mockBookService.Setup(service => service.Get(nonExistingBookId)).ReturnsAsync((Book)null);

            // Act
            var result = await _bookController.Delete(nonExistingBookId) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(404));
            _mockBookService.Verify(service => service.Delete(nonExistingBookId), Times.Never);
        }

        [Test]
        public async Task Delete_InvalidData_ReturnsBadRequest()
        {
            // Arrange
            var existingBook = _fixture.Create<Book>();
            var existingLoan = _fixture.Build<Loan>()
                                    .With(m => m.BookId, existingBook.Id)
                                    .With(m => m.Returned, false)
                                    .With(m => m.LoanDate, DateOnly.MinValue)
                                    .Create();

            var listOfLoans = new List<Loan>();
            listOfLoans.Add(existingLoan);
            _mockBookService.Setup(service => service.Get(existingBook.Id)).ReturnsAsync(existingBook);
            // Ne možemo obrisati knjigu ako ima aktivanu pozajmicu
            _mockLoanService.Setup(service => service.GetForBook(existingBook.Id)).ReturnsAsync(listOfLoans);

            // Act
            var result = await _bookController.Delete(existingBook.Id) as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(400));
            _mockBookService.Verify(service => service.Delete(existingBook.Id), Times.Never);
        }

        /////////////////////////////////GET BOOK///////////////////////////////////////////////

        [Test]
        public async Task GetBook_ValidId_ReturnsOkResult()
        {
            // Arrange
            var existingBook = _fixture.Create<Book>();
            _mockBookService.Setup(service => service.Get(existingBook.Id)).ReturnsAsync(existingBook);

            // Act
            var result = await _bookController.Get(existingBook.Id) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(existingBook));
        }

        [Test]
        public async Task GetBook_InvalidId_ReturnsNotFound()
        {
            // Arrange
            var nonExistingBookId = 999; // Ovaj ID ne postoji u bazi podataka
            _mockBookService.Setup(service => service.Get(nonExistingBookId)).ReturnsAsync((Book)null);

            // Act
            var result = await _bookController.Get(nonExistingBookId) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task GetBook_ServiceError_ReturnsInternalServerError()
        {
            // Arrange
            var existingBookId = 1; // Pretpostavimo da ovo postoji u bazi podataka
            _mockBookService.Setup(service => service.Get(existingBookId)).ThrowsAsync(new Exception("Simulated exception"));

            // Act
            var result = await _bookController.Get(existingBookId) as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(500));
            Assert.That(result.Value, Is.EqualTo("Internal Server Error"));
        }

        /////////////////////////////////GET BOOK TITLE///////////////////////////////////////////////

        [Test]
        public async Task GetTitle_ValidId_ReturnsOkResult()
        {
            // Arrange
            var existingBook = _fixture.Create<Book>();
            _mockBookService.Setup(service => service.Get(existingBook.Id)).ReturnsAsync(existingBook);

            // Act
            var result = await _bookController.GetTitle(existingBook.Id) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value.GetType().GetProperty("title").GetValue(result.Value, null), Is.EqualTo(existingBook.Title));
            Assert.That(result.Value.GetType().GetProperty("author").GetValue(result.Value, null), Is.EqualTo(existingBook.Author));
        }

        [Test]
        public async Task GetTitle_InvalidId_ReturnsNotFound()
        {
            // Arrange
            var nonExistingBookId = 999; // Ovaj ID ne postoji u bazi podataka
            _mockBookService.Setup(service => service.Get(nonExistingBookId)).ReturnsAsync((Book)null);

            // Act
            var result = await _bookController.GetTitle(nonExistingBookId) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task GetTitle_ServiceError_ReturnsInternalServerError()
        {
            // Arrange
            var existingBookId = 1; // Pretpostavimo da ovo postoji u bazi podataka
            _mockBookService.Setup(service => service.Get(existingBookId)).ThrowsAsync(new Exception("Simulated exception"));

            // Act
            var result = await _bookController.GetTitle(existingBookId) as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(500));
            Assert.That(result.Value, Is.EqualTo("Internal Server Error"));
        }
    }
}
