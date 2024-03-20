using LibraryAPI.Models;

namespace LibraryAPI.Services.Interfaces
{
    public interface ILoanService
    {
        Task<IEnumerable<Loan>> Get();
        Task<Loan> Get(int id);
        Task<Loan> Create( Loan loan);
        Task Update(int id, Loan loan);
        Task Delete(int id);
        Task<List<Loan>> GetForMember(int memberId);
        Task<List<Loan>> GetForBook(int bookId);
    }
}
