using Mapster;
using Library.Core.Repository;
using Library.Core.DTO;
using Library.Core.Helpers;
using Library.Core.Model;
using Library.Core.Common;
using Microsoft.Extensions.Logging.Abstractions;

namespace Library.Core.Service;

public class BookService : IBookService
{
    
    // refactoring the Program to Be serialized and Store in Json File instead of in-memory List collection 
    // by importing from repository to get the List of the Books and serializing it.
    // public static string filePath = "books.Json";
    private readonly IBookRepository _bookRepository;
    private readonly IUserRepository _userRepository;
    public BookService(IBookRepository bookRepository,IUserRepository userRepository)
    {
        this._bookRepository = bookRepository;        
        this._userRepository = userRepository;
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
    /*
        borrowbook method must take the user who wants to borrow and make business logic to the books he has borrowed and also 
        check whether the user are permitted to borrow or he would exceed the limit of the borrowed books  
    */ 

    

    // public Result<Book> BorrowBook(IBookService bookService, int id)
    // {
    //     var books = _bookRepository.GetAllBooks();
    //     var bookToBorrow = books.FirstOrDefault(book => book.Id == id);
 
    //     if(bookToBorrow is null)
    //             return Result<Book>.GenericFailure("This book does not exist.", 404);   

    //     if(!bookToBorrow.IsAvailable)
    //             return Result<Book>.GenericFailure("This is Book is Not Available.", 400);
            
    //     bookToBorrow?.IsAvailable = false;
    //     _bookRepository.SaveBooks(books);

    //     return Result<Book>.GenericSuccess(bookToBorrow, 200);
    // }

    // public Result<Book> returnBook(IBookService bookService, int id)
    // {
    //     var books = _bookRepository.GetAllBooks();
    //     var bookToReturn = books.FirstOrDefault(book => book.Id == id);
            
    //     if(bookToReturn is null)
    //             return Result<Book>.GenericFailure("This book does not exist.", 404);
        
    //     if(bookToReturn?.IsAvailable == true)
    //             return Result<Book>.GenericFailure("This book is Not Available", 400);

    //     bookToReturn?.IsAvailable = true;
    //     _bookRepository.SaveBooks(books);

    //     return Result<Book>.GenericSuccess(bookToReturn, 404);
    // }

    public Result<List<Book>> SearchBooks
    (string? Author = null, decimal? maxPrice = null)
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

    public BorrowResultStatus BorrowBook(int bookId, int userId)
    {
        var books = _bookRepository.GetAllBooks();
        var users = _userRepository.GetAllUsers();
        
        var bookToBorrow = books.FirstOrDefault(book => book.Id == bookId);
        var user = users.FirstOrDefault(user => user.Id == userId);

        if (bookToBorrow is null) return BorrowResultStatus.BookNotFound;
        if (!bookToBorrow.IsAvailable) return BorrowResultStatus.BookNotAvailable;
        if (user is null) return BorrowResultStatus.UserNotFound;
        if (user.BorrowedBooks >= 3) return BorrowResultStatus.UserLimitExceeded;

        user.BorrowedBooks += 1;
        user.Books.Add(bookToBorrow);
        bookToBorrow.IsAvailable = false;

        _bookRepository.SaveBooks(books);
        _userRepository.SaveUsers(users);

        return BorrowResultStatus.Success;
    }
    public ReturnResultStatus returnBook(int BookId, int UserId)
    {
        var books = _bookRepository.GetAllBooks();
        var users = _userRepository.GetAllUsers();
        var bookToReturn = books.FirstOrDefault(book => book.Id == BookId);
        var user = users.FirstOrDefault(user => user.Id == UserId); 
        var userBook = user?.Books?.FirstOrDefault(book => book.Id == BookId);

        if (bookToReturn is null) return ReturnResultStatus.BookNotFound;
        if (bookToReturn.IsAvailable) return ReturnResultStatus.BookAvailable;
        if (user is null) return ReturnResultStatus.UserNotFound;
        if(user.BorrowedBooks <= 0) return ReturnResultStatus.UserHasNoBorrowedBooks;
        if(userBook is null) return ReturnResultStatus.UserBookNotFound;
        
        user.BorrowedBooks -= 1;
        user.Books.Remove(userBook);
        bookToReturn.IsAvailable = true;

        _bookRepository.SaveBooks(books);
        _userRepository.SaveUsers(users);
        
        return ReturnResultStatus.Success;
    }
}