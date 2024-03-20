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
    public class MemberControllerTest
    {
        private MemberController _memberController;
        private Mock<IMemberService> _mockMemberService;
        private Mock<ILoanService> _mockLoanService;
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _mockMemberService = new Mock<IMemberService>();
            _mockLoanService = new Mock<ILoanService>();
            _memberController = new MemberController(_mockMemberService.Object, _mockLoanService.Object);
        }

        /////////////////////////////////GET ALL MEMBERS///////////////////////////////////////////////

        [Test]
        public async Task GetAllMembers_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var members = _fixture.Build<Member>()
                                    .With(m => m.MembershipExpiration, DateOnly.MinValue)
                                    .CreateMany(3); // Generišemo nekoliko clanova

            _mockMemberService.Setup(service => service.Get()).ReturnsAsync(members);

            // Act
            var result = await _memberController.Get() as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(members));
        }


        [Test]
        public async Task GetAllMembers_NoMembers_ReturnsNotFound()
        {
            // Arrange
            IEnumerable<Member> members = null;

            _mockMemberService.Setup(service => service.Get()).ReturnsAsync(members);

            // Act
            var result = await _memberController.Get() as NotFoundResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(404));
        }


        [Test]
        public async Task GetAllMembers_ServiceError_ReturnsInternalServerError()
        {
            // Arrange
            _mockMemberService.Setup(service => service.Get()).ThrowsAsync(new Exception("Simulated exception"));

            // Act
            var result = await _memberController.Get() as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(500));
            Assert.That(result.Value, Is.EqualTo("Internal Server Error"));
        }

        /////////////////////////////////ADD MEMBER///////////////////////////////////////////////

        [Test]
        public async Task Post_ValidMember_ReturnsOkResult()
        {
            // Arrange
            var newMember = _fixture.Build<Member>()
                                    .With(m => m.MembershipExpiration, DateOnly.MinValue)
                                    .Create();

            // Act
            var result = await _memberController.Post(newMember) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(newMember));
        }


        [Test]
        public async Task Post_InvalidMember_ReturnsBadRequest()
        {
            // Arrange
            var invalidMember = _fixture.Build<Member>()
                                        .Without(m => m.Firstname)
                                        .With(m => m.MembershipExpiration, DateOnly.MinValue) // Random datum isteka članstva
                                        .Create();

            // Act
            var result = await _memberController.Post(invalidMember) as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(400));
        }

        [Test]
        public async Task Post_InvalidMember2_ReturnsBadRequest()
        {
            // Arrange
            var invalidMember = _fixture.Build<Member>()
                                        .With(m => m.Age,0)
                                        .With(m => m.MembershipExpiration, DateOnly.MinValue) // Random datum isteka članstva
                                        .Create();

            // Act
            var result = await _memberController.Post(invalidMember) as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(400));
        }

        [Test]
        public async Task Post_EmptyMember_ReturnsBadRequest()
        {
            // Arrange 
            var newMember = new Member();

            // Act
            var result = await _memberController.Post(newMember) as BadRequestObjectResult;

            // Assert+
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(400));
        }
        [Test]
        public async Task Post_ServiceError_ReturnsInternalServerError()
        {
            // Arrange
            _mockMemberService.Setup(service => service.Create(It.IsAny<Member>())).ThrowsAsync(new Exception("Simulated exception"));
            var newMember = new Member { Firstname = "Jana", Lastname = "Stojanovic", Age = 23 };

            // Act
            var result = await _memberController.Post(newMember) as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(500));
            Assert.That(result.Value, Is.EqualTo("Internal Server Error"));
        }

        /////////////////////////////////EDIT MEMBER///////////////////////////////////////////////

        [Test]
        public async Task Update_ValidMember_ReturnsOkResult()
        {
            // Arrange
            var existingMember = _fixture.Build<Member>()
                                    .With(m => m.MembershipExpiration, DateOnly.MinValue)
                                    .Create();
            var updatedMember = _fixture.Build<Member>()
                                    .With(m => m.MembershipExpiration, DateOnly.MinValue)
                                    .Create();

            _mockMemberService.Setup(service => service.Get(existingMember.Id)).ReturnsAsync(existingMember);

            // Act
            var result = await _memberController.Update(existingMember.Id, updatedMember) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(updatedMember));
            _mockMemberService.Verify(service => service.Update(existingMember.Id, updatedMember), Times.Once);
        }

        [Test]
        public async Task Update_InvalidMember_ReturnsNotFound()
        {
            // Arrange
            var nonExistingMemberId = 999; // Ovaj ID ne postoji u bazi podataka
            var updatedMember = _fixture.Build<Member>()
                                    .With(m => m.MembershipExpiration, DateOnly.MinValue)
                                    .Create();

            _mockMemberService.Setup(service => service.Get(nonExistingMemberId)).ReturnsAsync((Member)null);

            // Act
            var result = await _memberController.Update(nonExistingMemberId, updatedMember) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(404));
            _mockMemberService.Verify(service => service.Update(nonExistingMemberId, updatedMember), Times.Never);
        }


        [Test]
        public async Task Update_InvalidData_ReturnsBadRequest()
        {
            // Arrange
            var existingMember = _fixture.Build<Member>()
                                    .With(m => m.MembershipExpiration, DateOnly.MinValue)
                                    .Create();
            var updatedMember = _fixture.Build<Member>()
                                        .Without(m => m.Firstname)
                                        .With(m => m.MembershipExpiration, DateOnly.MinValue) // RANDOM datum isteka članstva
                                        .Create(); 

            _mockMemberService.Setup(service => service.Get(existingMember.Id)).ReturnsAsync(existingMember);

            // Act
            var result = await _memberController.Update(existingMember.Id, updatedMember) as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(400));
            _mockMemberService.Verify(service => service.Update(existingMember.Id, updatedMember), Times.Never);
        }
        [Test]
        public async Task Update_InvalidData2_ReturnsBadRequest()
        {
            // Arrange
            var existingMember = _fixture.Build<Member>()
                                    .With(m => m.MembershipExpiration, DateOnly.MinValue)
                                    .Create();
            var updatedMember = _fixture.Build<Member>()
                                        .With(m => m.Age,0)
                                        .With(m => m.MembershipExpiration, DateOnly.MinValue) // RANDOM datum isteka članstva
                                        .Create(); 
            _mockMemberService.Setup(service => service.Get(existingMember.Id)).ReturnsAsync(existingMember);

            // Act
            var result = await _memberController.Update(existingMember.Id, updatedMember) as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(400));
            _mockMemberService.Verify(service => service.Update(existingMember.Id, updatedMember), Times.Never);
        }

        /////////////////////////////////DELETE MEMBER///////////////////////////////////////////////

        [Test]
        public async Task Delete_ValidId_ReturnsOkResult()
        {
            // Arrange
            var existingMember = _fixture.Build<Member>()
                                    .With(m => m.MembershipExpiration, DateOnly.MinValue)
                                    .Create();

            _mockMemberService.Setup(service => service.Get(existingMember.Id)).ReturnsAsync(existingMember);

            // Act
            var result = await _memberController.Delete(existingMember.Id) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(existingMember));
            _mockMemberService.Verify(service => service.Delete(existingMember.Id), Times.Once);
        }

        [Test]
        public async Task Delete_InvalidId_ReturnsNotFound()
        {
            // Arrange
            var nonExistingMemberId = 999; // Ovaj ID ne postoji u bazi podataka

            _mockMemberService.Setup(service => service.Get(nonExistingMemberId)).ReturnsAsync((Member)null);

            // Act
            var result = await _memberController.Delete(nonExistingMemberId) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(404));
            _mockMemberService.Verify(service => service.Delete(nonExistingMemberId), Times.Never);
        }

        [Test]
        public async Task Delete_InvalidData_ReturnsBadRequest()
        {
            // Arrange
            var existingMember = _fixture.Build<Member>()
                                    .With(m => m.MembershipExpiration, DateOnly.MinValue)
                                    .Create();
            var existingLoan = _fixture.Build<Loan>()
                                    .With(m => m.MemberId, existingMember.Id)
                                    .With(m => m.Returned, false)
                                    .With(m => m.LoanDate, DateOnly.MinValue)
                                    .Create();

            var listOfLoans = new List<Loan>();
            listOfLoans.Add(existingLoan);
            _mockMemberService.Setup(service => service.Get(existingMember.Id)).ReturnsAsync(existingMember);
            // Ne možemo obrisati člana ako ima aktivanu pozajmicu
            _mockLoanService.Setup(service => service.GetForMember(existingMember.Id)).ReturnsAsync(listOfLoans);

            // Act
            var result = await _memberController.Delete(existingMember.Id) as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(400));
            _mockMemberService.Verify(service => service.Delete(existingMember.Id), Times.Never);
        }

        /////////////////////////////////GET MEMBER///////////////////////////////////////////////

        [Test]
        public async Task GetMember_ValidId_ReturnsOkResult()
        {
            // Arrange
            var existingMember = _fixture.Build<Member>()
                                    .With(m => m.MembershipExpiration, DateOnly.MinValue)
                                    .Create();
            _mockMemberService.Setup(service => service.Get(existingMember.Id)).ReturnsAsync(existingMember);

            // Act
            var result = await _memberController.Get(existingMember.Id) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(existingMember));
        }

        [Test]
        public async Task GetMember_InvalidId_ReturnsNotFound()
        {
            // Arrange
            var nonExistingMemberId = 999; // Ovaj ID ne postoji u bazi podataka
            _mockMemberService.Setup(service => service.Get(nonExistingMemberId)).ReturnsAsync((Member)null);

            // Act
            var result = await _memberController.Get(nonExistingMemberId) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task GetMember_ServiceError_ReturnsInternalServerError()
        {
            // Arrange
            var existingMemberId = 1; // Pretpostavimo da ovo postoji u bazi podataka
            _mockMemberService.Setup(service => service.Get(existingMemberId)).ThrowsAsync(new Exception("Simulated exception"));

            // Act
            var result = await _memberController.Get(existingMemberId) as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(500));
            Assert.That(result.Value, Is.EqualTo("Internal Server Error"));
        }

        /////////////////////////////////GET MEMBER NAME///////////////////////////////////////////////

        [Test]
        public async Task GetName_ValidId_ReturnsOkResult()
        {
            // Arrange
            var existingMember = _fixture.Build<Member>()
                                     .With(m => m.MembershipExpiration, DateOnly.MinValue)
                                     .Create();
            _mockMemberService.Setup(service => service.Get(existingMember.Id)).ReturnsAsync(existingMember);

            // Act
            var result = await _memberController.GetName(existingMember.Id) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value.GetType().GetProperty("name").GetValue(result.Value, null), Is.EqualTo(existingMember.Firstname));
            Assert.That(result.Value.GetType().GetProperty("surname").GetValue(result.Value, null), Is.EqualTo(existingMember.Lastname));
        }

        [Test]
        public async Task GetName_InvalidId_ReturnsNotFound()
        {
            // Arrange
            var nonExistingMemberId = 999; // Ovaj ID ne postoji u bazi podataka
            _mockMemberService.Setup(service => service.Get(nonExistingMemberId)).ReturnsAsync((Member)null);

            // Act
            var result = await _memberController.GetName(nonExistingMemberId) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task GetName_ServiceError_ReturnsInternalServerError()
        {
            // Arrange
            var existingMemberId = 1; // Pretpostavimo da ovo postoji u bazi podataka
            _mockMemberService.Setup(service => service.Get(existingMemberId)).ThrowsAsync(new Exception("Simulated exception"));

            // Act
            var result = await _memberController.GetName(existingMemberId) as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(500));
            Assert.That(result.Value, Is.EqualTo("Internal Server Error"));
        }

        /////////////////////////////////EXTEND MEMBERSHIP///////////////////////////////////////////////

        [Test]
        public async Task UpdateMembership_ValidId_ReturnsOkResult()
        {
            // Arrange
            var existingMember = _fixture.Build<Member>()
                                    .With(m => m.MembershipExpiration, DateOnly.MinValue)
                                    .Create();
            var updatedMembershipExpiration = DateOnly.FromDateTime(DateTime.Now.AddYears(1));

            _mockMemberService.Setup(service => service.Get(existingMember.Id)).ReturnsAsync(existingMember);

            // Act
            var result = await _memberController.UpdateMembership(existingMember.Id) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(((Member)result.Value).Id, Is.EqualTo(existingMember.Id));
            Assert.That(((Member)result.Value).Firstname, Is.EqualTo(existingMember.Firstname));
            Assert.That(((Member)result.Value).Lastname, Is.EqualTo(existingMember.Lastname));
            Assert.That(((Member)result.Value).Email, Is.EqualTo(existingMember.Email));
            Assert.That(((Member)result.Value).Age, Is.EqualTo(existingMember.Age));
            Assert.That(((Member)result.Value).MembershipExpiration, Is.EqualTo(updatedMembershipExpiration));
        }

        [Test]
        public async Task UpdateMembership_InvalidId_ReturnsNotFound()
        {
            // Arrange
            var nonExistingMemberId = 999; // Ovaj ID ne postoji u bazi podataka
            _mockMemberService.Setup(service => service.Get(nonExistingMemberId)).ReturnsAsync((Member)null);

            // Act
            var result = await _memberController.UpdateMembership(nonExistingMemberId) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task UpdateMembership_ServiceError_ReturnsInternalServerError()
        {
            // Arrange
            var existingMemberId = 1; // Pretpostavimo da ovo postoji u bazi podataka
            _mockMemberService.Setup(service => service.Get(existingMemberId)).ThrowsAsync(new Exception("Simulated exception"));

            // Act
            var result = await _memberController.UpdateMembership(existingMemberId) as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(500));
            Assert.That(result.Value, Is.EqualTo("Internal Server Error"));
        }


    }
}
