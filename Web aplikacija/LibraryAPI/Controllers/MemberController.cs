using LibraryAPI.Models;
using LibraryAPI.Services;
using LibraryAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemberController : ControllerBase
    {
        private readonly IMemberService _memberService;
        private readonly ILoanService _loanService;

        public MemberController(IMemberService memberService, ILoanService loanService)
        {
            _memberService = memberService;
            _loanService = loanService;
        }

        [HttpGet]
        [Route("GetAllMembers")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var members = await _memberService.Get();
                if (members == null)
                {
                    return NotFound();
                }
                return Ok(members);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet]
        [Route("GetMember/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var memeber = await _memberService.Get(id);

                if (memeber is null)
                {
                    return NotFound();
                }

                return Ok(memeber);

            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }
        [HttpGet]
        [Route("GetMemberName/{id}")]
        public async Task<IActionResult> GetName(int id)
        {
            try
            {
                var memeber = await _memberService.Get(id);

                if (memeber is null)
                {
                    return NotFound();
                }
                var name = memeber.Firstname + " " + memeber.Lastname;

                return Ok(new { name = memeber.Firstname, surname = memeber.Lastname });
            }

            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost]
        [Route("AddMember")]
        public async Task<IActionResult> Post(Member newMember)
        {

            try
            {
                if (newMember == null || string.IsNullOrWhiteSpace(newMember.Firstname) || string.IsNullOrWhiteSpace(newMember.Lastname))
                {
                    return BadRequest(new
                    {
                        ErrorMessage = "Firstname and lastname fields are required"
                    });
                }

                if (newMember.Age == 0)
                {
                    return BadRequest(new
                    {
                        ErrorMessage = "Invalid age"
                    });
                }

                newMember.MembershipExpiration = DateOnly.FromDateTime(DateTime.Now.AddMonths(12));
                await _memberService.Create(newMember);

                return Ok(newMember);

            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPut]
        [Route("EditMember/{id}")]
        public async Task<IActionResult> Update(int id, Member updatedMember)
        {
            var memeber = await _memberService.Get(id);
            if (memeber is null)
            {
                return NotFound();
            }

            if (updatedMember == null || string.IsNullOrWhiteSpace(updatedMember.Firstname) || string.IsNullOrWhiteSpace(updatedMember.Lastname))
            {
                return BadRequest(new
                {
                    ErrorMessage = "Firstname and lastname fields are required"
                });
            }
            if (updatedMember.Age == 0)
            {
                return BadRequest(new
                {
                    ErrorMessage = "Invalid age"
                });
            }
            updatedMember.Id = id;
            updatedMember.MembershipExpiration = memeber.MembershipExpiration;
            await _memberService.Update(id, updatedMember);

            return Ok(updatedMember);
        }

        [HttpPut]
        [Route("ExtendMembership/{id}")]
        public async Task<IActionResult> UpdateMembership(int id)
        {
            try
            {
                var updatedMember = new Member();
                var memeber = await _memberService.Get(id);
                if (memeber is null)
                {
                    return NotFound();
                }
                updatedMember.Id = id;
                updatedMember.Firstname = memeber.Firstname;
                updatedMember.Lastname = memeber.Lastname;
                updatedMember.Email = memeber.Email;
                updatedMember.Age = memeber.Age;
                updatedMember.MembershipExpiration = DateOnly.FromDateTime(DateTime.Now.AddYears(1));
                await _memberService.Update(id, updatedMember);

                return Ok(updatedMember);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpDelete]
        [Route("DeleteMember/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var memeber = await _memberService.Get(id);
            if (memeber is null)
            {
                return NotFound();
            }

            var loans = await _loanService.GetForMember(id);
            if (loans != null)
            {
                foreach (var loan in loans)
                {
                    if (loan.Returned == false)
                    {
                        return BadRequest("Can't delete member as long as he have loans that are not returned");
                    }
                }
            }

            await _memberService.Delete(id);

            return Ok(memeber);
        }
    }
}
