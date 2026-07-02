using Mapster;
using Practice.Repository;
using Practice.DTO;
using Practice.Helpers;
using Practice.Model;
using Practice.Common;

namespace Practice.Service;

public class BookService : IBookService
{
    
    // refactoring the Program to Be serialized and Store in Json File instead of in-memory List collection 
    // by importing from repository to get the List of the Books and serializing it.
    // public static string filePath = "books.Json";
    private readonly IBookRepository _bookRepository;
    public BookService(IBookRepository bookRepository)
    {
        this._bookRepository = bookRepository;        
    }
    public List<Book> GetAllBooks()
    {
        return _bookRepository.GetAllBooks();
    }
    
    public Book? GetBookByID(int id)  
    {
        var _books = _bookRepository.GetAllBooks();
        var bookToReturn = _books.FirstOrDefault(book => book.Id == id);
        return bookToReturn;
    }

    public Book CreateBook(CreateBookDto createBookDto)
    {
        var _books = _bookRepository.GetAllBooks();
        var book = createBookDto.Adapt<Book>();
        book.Id = _books.Any() ? _books.Max(x => x.Id) + 1 : 1;
        book.SerialNumber = SerialGenerator.GenerateSerialNumber();
        _books.Add(book);
        _bookRepository.SaveBooks(_books);
        return book;
    }

    public bool DeleteBook(int id)
    {
        var _books = _bookRepository.GetAllBooks();
        var bookToDelete = _books.FirstOrDefault(book => book.Id == id);
        if(bookToDelete is null)
            return false;

        _books.Remove(bookToDelete);
        _bookRepository.SaveBooks(_books);
        return true;
    }
    public bool UpdateBook(CreateBookDto createBookDto, int id)
    {
        var _books = _bookRepository.GetAllBooks();
        var bookToUpdate = _books.FirstOrDefault(book => book.Id == id);
        if(bookToUpdate is null)
            return false;
        
        createBookDto.Adapt(bookToUpdate);
        _bookRepository.SaveBooks(_books);
        return true;
    }

    public List<Book> Pagination(int pageNo, int pageSize)
    {
        if(pageNo <= 0)
            pageNo = 1;

        if(pageSize <= 0)
            pageSize = 10;

        var books = _bookRepository.GetAllBooks().
                    Skip((pageNo - 1) * pageSize).
                    Take(pageSize);
        
        return books.ToList();
    }

    public Result<Book> BorrowBook(IBookService bookService, int id)
    {
        var books = _bookRepository.GetAllBooks();
        var bookToBorrow = books.FirstOrDefault(book => book.Id == id);
 
        if(bookToBorrow is null)
                return Result<Book>.GenericFailure("This book does not exist.", 404);   

        if(!bookToBorrow.IsAvailable)
                return Result<Book>.GenericFailure("This is Book is Not Available.", 400);
            
        bookToBorrow?.IsAvailable = false;
        _bookRepository.SaveBooks(books);

        return Result<Book>.GenericSuccess(bookToBorrow, 200);
    }

    public Result<Book> returnBook(IBookService bookService, int id)
    {
        var books = _bookRepository.GetAllBooks();
        var bookToReturn = books.FirstOrDefault(book => book.Id == id);
            
        if(bookToReturn is null)
                return Result<Book>.GenericFailure("This book does not exist.", 404);
        
        if(bookToReturn?.IsAvailable == true)
                return Result<Book>.GenericFailure("This book is Not Available", 400);

        bookToReturn?.IsAvailable = true;
        _bookRepository.SaveBooks(books);

        return Result<Book>.GenericSuccess(bookToReturn, 404);
    }

    public Result<List<Book>> SearchBooks
    (IBookService bookService, string? Author = null, decimal? maxPrice = null)
    {
        var books = _bookRepository.GetAllBooks().ToList();
        var filteredBooks = books;

        if(!string.IsNullOrEmpty(Author))        
        {
            filteredBooks = books.Where(book => 
                book.Author.Contains(Author, StringComparison.OrdinalIgnoreCase)).ToList();
        }
        
        if(maxPrice.HasValue)
        {
            filteredBooks = filteredBooks.Where(book => book.Price <= maxPrice).ToList();
        }

        if(filteredBooks is null)
        {
            return Result<List<Book>>.GenericFailure("Book Not Found", 404);
        }
        
        return Result<List<Book>>.GenericSuccess(filteredBooks, 200);
    }
}