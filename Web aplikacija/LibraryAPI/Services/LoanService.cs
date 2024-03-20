using LibraryAPI.Data;
using LibraryAPI.Models;
using LibraryAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryAPI.Services
{
    public class LoanService:ILoanService
    {
        private readonly ApplicationDbContext _context;

        public LoanService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Loan> Create(Loan loan)
        {
            _context.Loans.Add(loan);
            await _context.SaveChangesAsync();

            return loan;
        }

        public async Task Delete(int id)
        {
            var loanToDelete = await _context.Loans.AsNoTracking().Where(x => x.Id == id).FirstOrDefaultAsync();
            _context.Loans.Remove(loanToDelete);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Loan>> Get()
        {
            return await _context.Loans.ToListAsync();
        }

        public async Task<Loan> Get(int id)
        {
            return await _context.Loans.AsNoTracking().Where(x=>x.Id==id).FirstOrDefaultAsync();
        }

        public async Task Update(int id,Loan loan)
        {
            _context.Entry(loan).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
        public async Task<List<Loan>> GetForMember(int memberId)
        {
            var loans = await _context.Loans.Where(k=>k.MemberId == memberId).ToListAsync();
            return loans;
        }

        public async Task<List<Loan>> GetForBook(int bookId)
        {
            var loans = await _context.Loans.Where(k => k.BookId == bookId).ToListAsync();
            return loans;
        }
    }
}
