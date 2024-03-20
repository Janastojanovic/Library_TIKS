using LibraryAPI.Models;

namespace LibraryAPI.Services.Interfaces
{
    public interface IBookService
    {
            Task<IEnumerable<Book>> Get();
            Task<Book> Get(int id);
            Task<Book> Create(Book book);
            Task Update(int id,Book book);
            Task Delete(int id);
    }
}
