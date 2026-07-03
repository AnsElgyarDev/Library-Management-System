using Practice.DTO;
using Practice.Model;

namespace Library.Core.Service;

public interface IUserBooksServices
{
    UserBooksDto GetUserBooks(int id);
    Book DeleteUserBooks(int UserId, int BookId);
    CreateBookDto UpdateUserBooks(int UserId, int BookId, CreateBookDto createBookDto);
    CreateBookDto CreateUserBooks(int UserId, CreateBookDto createBookDto);
}