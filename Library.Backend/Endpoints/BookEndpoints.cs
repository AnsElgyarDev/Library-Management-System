using Microsoft.AspNetCore.Http.HttpResults;
using Library.Core.Common;
using Library.Core.DTO;
using Library.Core.Model;
using Library.Core.Service;
using Library.Core.Validators;

namespace Library.Core.Endpoints;
public static class Endpoints
{   
    public static void MapBookEndpoints(this WebApplication app)
    {
        // inject all the bookRepo in the parameters and get all in the parameter for the service 
        // Get Methods
        app.MapGet("/", () => "Welcome To Library Management System");

        app.MapGet("/books", Results<NotFound<string>, Ok<List<Book>>> 
                  (IBookService bookService) =>
        {
            var books = bookService.GetAllBooks();
            if(!books.Any())
            {
                return TypedResults.NotFound("No Books Avalilable!");
            }
            
            return TypedResults.Ok(books);    
        });
        app.MapGet("/books/{pageNo}/{pageSize}", Results<NotFound, Ok<List<Book>>>
                  (IBookService bookService, int pageNo, int pageSize) =>
        {
            var books = bookService.Pagination(pageNo, pageSize);
            if(books is null)
                return TypedResults.NotFound();        
            return TypedResults.Ok(books); 
        });
        
        app.MapGet("/books/{id}",  Results<NotFound<string>, Ok<Book>>
                  (IBookService bookService, int id) =>
        {
            var book = bookService.GetBookByID(id);
            if(book is null)
                return TypedResults.NotFound("Not Found");

            return TypedResults.Ok(book);
        });

        app.MapGet("/books/search", Results<NotFound, Ok<Result<List<Book>>>>
        (IBookService bookService, string? Author, decimal? maxPrice) =>
        {
            var searchedBooks = bookService.SearchBooks(bookService, Author, maxPrice);
            if(searchedBooks is null)
                return TypedResults.NotFound();

            return TypedResults.Ok(searchedBooks);
        });

        // Post Methods
        app.MapPost("/books", (IBookService bookService, CreateBookDto createBookDto) =>
        {
            var book = bookService.CreateBook(createBookDto);
            return TypedResults.Created($"/books/{book.Id}", book);
        }).AddEndpointFilter<ValidationFilter<CreateBookDto>>();

        app.MapPost("/books/{id}/borrow", (IBookService bookService, int id) =>
        {
            var result = bookService.BorrowBook(bookService, id);
            
            if(!result.isSuccess)
            {
                return Results.Json(new
                {
                     error = result.ErrorMessage
                }, statusCode: result.StatusCode);
            }

            return Results.Ok(result.Data);
        });

        app.MapPost("/books/{id}/return", (IBookService bookService, int id) =>
        {
            var result = bookService.returnBook(bookService, id);
            
            if(!result.isSuccess)
            {
                return Results.Json(new
                {
                     error = result.ErrorMessage
                }, statusCode: result.StatusCode);
            }

            return Results.Ok(result.Data);
        });

        // Put Methods
        app.MapPut("/books/{id}", Results<NoContent, NotFound>
                  (IBookService bookService, CreateBookDto createBookDto, int id) =>
        {
            bool isSuccess = bookService.UpdateBook(createBookDto, id);
            return isSuccess ? TypedResults.NoContent() : TypedResults.NotFound();
        }).AddEndpointFilter<ValidationFilter<CreateBookDto>>();
        
        // Delete Methods 
        app.MapDelete("/books/{id}", Results<NoContent, NotFound> 
                     (IBookService bookService, int id) =>
        {
            return bookService.DeleteBook(id) ?  TypedResults.NoContent() : TypedResults.NotFound();
        }); 

    }
} 
