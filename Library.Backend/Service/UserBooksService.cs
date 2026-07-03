using Mapster;
using Practice.DTO;
using Practice.Model;
using Practice.Repository;

namespace Library.Core.Service;

public class UserBooksService : IUserBooksServices
{
    private readonly IUserRepository _userRepository;
    public UserBooksService(IUserRepository userRepository)
    {
        this._userRepository = userRepository;
    }

    public Book DeleteUserBooks(int Userid, int BookId)
    {
        var users = _userRepository.GetAllUsers(); 
        var user = users.FirstOrDefault(user => user.Id == Userid);
        
        if(user is null)
            return null!;
        var books = user.Books;
        var bookToRemove = books.FirstOrDefault(book => book.Id == BookId);
        if(bookToRemove is null)
            return null!;
        books.Remove(bookToRemove);
        _userRepository.SaveUsers(users);
        return bookToRemove;
    }

    public UserBooksDto GetUserBooks(int id)
    {
        var user = _userRepository.GetAllUsers().FirstOrDefault(user => user.Id == id);
        
        if(user is null)
            return null!;
        var userToReturn = user.Adapt<UserBooksDto>();
        
        return userToReturn;
    }

    public CreateBookDto UpdateUserBooks(int UserId, int BookId, CreateBookDto bookDto)
    {
        var users = _userRepository.GetAllUsers().FirstOrDefault(user => user.Id == UserId);
        
        if(users is null)
            return null!;
        
        var bookToUpdate = users.Books.FirstOrDefault(book => book.Id == BookId);
        bookToUpdate.Adapt(bookDto);
        
        return bookDto;
    }
    public CreateBookDto CreateUserBooks(int UserId, CreateBookDto createBookDto)
    {
        var users = _userRepository.GetAllUsers();
        var user = users.FirstOrDefault(user => user.Id == UserId);
        if(user is null)
            return null!;

        var books = user.Books; 
        Book bookToAdd = createBookDto.Adapt<Book>();  
        user.Books.Add(bookToAdd);
        _userRepository.SaveUsers(users);      
        return createBookDto;
    }
}