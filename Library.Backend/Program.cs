using FluentValidation;
using Library.Core.Endpoints;
using Library.Core.Middlewares;
using Library.Core.Repository;
using Library.Core.Service;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// registering Services 
/*
    user services and their implemenation and then injected and implemented the endpoints properly
*/ 
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails(); 
builder.Services.AddSingleton<IBookService, BookService>(); 
builder.Services.AddScoped<IUserBooksServices, UserBooksService>(); 
builder.Services.AddScoped<IUserService, UserService>(); 
builder.Services.AddScoped<IUserRepository, UserRepository>(); 
builder.Services.AddValidatorsFromAssemblyContaining<Program>(); 
builder.Services.AddSingleton<IBookRepository, BookRepository>();
builder.Services.AddOpenApi();
// builder.Services.AddAuthentication();
// builder.Services.AddAuthorization();

var app = builder.Build();

// adding Middlewares
if(app.Environment.IsDevelopment())
{
    app.MapOpenApi(); // to transfer the Endpoints
    app.MapScalarApiReference(); // to create the UI 
}

// used first to catch any error happens on the server wraps the rest of the App in a try-catch block
app.UseExceptionHandler();

// used for the security staff converting to HTTPS
// app.UseHttpsRedirection(); 

// used to match the incoming HTTP request by the URL Path and the HTTP Verb to Exact Endpoints
app.UseRouting();

// used to determine whether the user was authenticated or not by having token in the request header to enter the app or not
// app.UseAuthentication();

// used for authorizing for some warrants in the Program and like admin role 
// app.UseAuthorization();    

// Custom Middleware returns the request path and the response status code.
app.UseMiddleware<RequestLogMiddleware>();  

// app.UseMiddleware<AuthorizationMiddleware>();

// adding endpoints reference
app.MapUserEndpoints();
app.MapBookEndpoints();
app.MapUserBooksEndpoints();

app.Run();      