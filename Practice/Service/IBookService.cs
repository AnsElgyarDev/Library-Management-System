using Practice.Common;
using Practice.DTO;
using Practice.Model;
using Practice.Repository;

namespace Practice.Service;

public interface IBookService
{
    List<Book> GetAllBooks();
    Book? GetBookByID(int id);
    Book CreateBook(CreateBookDto createBookDto);
    bool DeleteBook(int id);
    bool UpdateBook(CreateBookDto createBookDto, int id);
    List<Book> Pagination(int pageNo, int pageSize);
    Result<Book> BorrowBook(IBookService bookService, int id);
    Result<Book> returnBook(IBookService bookService, int id);
    Result<List<Book>> SearchBooks(IBookService bookService, string? Author = null, decimal? maxPrice = null);
}