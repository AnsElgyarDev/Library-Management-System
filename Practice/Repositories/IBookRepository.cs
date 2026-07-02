using Practice.Model;

namespace Practice.Repository;

public interface IBookRepository
{
    List<Book> GetAllBooks();
    void SaveBooks(List<Book> books);
}