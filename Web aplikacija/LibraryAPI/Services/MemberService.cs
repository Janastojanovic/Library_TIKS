using LibraryAPI.Data;
using LibraryAPI.Models;
using LibraryAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryAPI.Services
{
    public class MemberService: IMemberService
    {
        private readonly ApplicationDbContext _context;

        public MemberService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Member> Create(Member member)
        {
            _context.Members.Add(member);
            await _context.SaveChangesAsync();

            return member;
        }

        public async Task Delete(int id)
        {
            var memberToDelete = await _context.Members.AsNoTracking().Where(x => x.Id == id).FirstOrDefaultAsync();
            _context.Members.Remove(memberToDelete);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Member>> Get()
        {
            return await _context.Members.ToListAsync();
        }

        public async Task<Member> Get(int id)
        {
            return await _context.Members.AsNoTracking().Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task Update(int id,Member member)
        {
            _context.Entry(member).State = EntityState.Modified;
            await _context.SaveChangesAsync();

        }

    }
}
