using LibraryAPI.Models;

namespace LibraryAPI.Services.Interfaces
{
    public interface IMemberService
    {
            Task<IEnumerable<Member>> Get();
            Task<Member> Get(int id);
            Task<Member> Create(Member member);
            Task Update(int id, Member member);
            Task Delete(int id);
    }
}
