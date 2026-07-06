using Library.Core.Common;
using Library.Core.DTO;
using Library.Core.Model;
using Library.Core.Repository;

namespace Library.Core.Service;

public interface IBookService
{
    List<Book> GetAllBooks();
    Book? GetBookByID(int id);
    Book CreateBook(CreateBookDto createBookDto);
    bool DeleteBook(int id);
    bool UpdateBook(CreateBookDto createBookDto, int id);
    List<Book> Pagination(int pageNo, int pageSize);
    BorrowResultStatus BorrowBook(int BookId, int UserId);
    ReturnResultStatus returnBook(int BookId, int UserId);
    Result<List<Book>> SearchBooks(string? Author = null, decimal? maxPrice = null);
}