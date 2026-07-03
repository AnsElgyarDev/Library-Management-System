using Practice.Model;

namespace Library.Core.Repository;

public interface IBookRepository
{
    List<Book> GetAllBooks();
    void SaveBooks(List<Book> books);
}