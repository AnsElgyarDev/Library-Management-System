using Microsoft.AspNetCore.Http.HttpResults;
using Practice.DTO;
using Practice.Model;
using Practice.Service;

namespace Practice.Endpoints;

public static class UserBooksEndpoints
{
    public static void MapUserBooksEndpoints(this WebApplication app)
    {
        app.MapGet("/User/{UserId}/Books", Results<NotFound<string>, Ok<UserBooksDto>>
        (IUserBooksServices userBookService, int UserId) =>
        {
            var userBooks = userBookService.GetUserBooks(UserId);
            if(userBooks is null)
                return TypedResults.NotFound("Not Found!");
            
            return TypedResults.Ok(userBooks);
        });
        
        app.MapDelete("/Users/{UserId}/Books/{BookId}", Results<NotFound<string>, Ok<Book>> 
        (IUserBooksServices userBooksServices, int UserId, int BookId) =>
        {
            var userBook = userBooksServices.DeleteUserBooks(UserId, BookId);
            if(userBook is null)
                return TypedResults.NotFound("Not Found!");
            
            return TypedResults.Ok(userBook);
        });

        app.MapPut("/Users/{UserId}/Books/{BookId}", Results<NotFound<string>, Ok<CreateBookDto>> 
        (IUserBooksServices userBooksServices, int UserId, int BookId, CreateBookDto createBookDto) =>
        {
            var UserBook = userBooksServices.UpdateUserBooks(UserId, BookId, createBookDto);   
            if(UserBook is null)
                return TypedResults.NotFound("Not Found!");
                
            return TypedResults.Ok(UserBook);
        });

        app.MapPost("/Users/{UserId}/Books/", Results<NotFound<string>, Ok<CreateBookDto>> 
        (IUserBooksServices userBooksServices, int UserId, CreateBookDto createBookDto) =>
        {
            var UserBook = userBooksServices.CreateUserBooks(UserId, createBookDto);   
            if(UserBook is null)
                return TypedResults.NotFound("Not Found!");
            
            return TypedResults.Ok(UserBook);
        });
    }
} 