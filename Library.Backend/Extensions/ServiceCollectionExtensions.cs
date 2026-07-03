using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Library.Core.Repository;
using Library.Core.Service;
using Library.Core.Validators;

namespace Library.Core.Extensions;

// This lives INSIDE the Practice assembly on purpose: BookRepository and
// UserRepository are declared "internal", so only code inside this assembly
// can reference the concrete types. Consumers (like LibraryWinForms) just
// call services.AddPracticeServices() and never need to know those types exist.
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPracticeServices(this IServiceCollection services)
    {
        services.AddSingleton<IBookRepository, BookRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        services.AddSingleton<IBookService, BookService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserBooksServices, UserBooksService>();

        services.AddValidatorsFromAssemblyContaining<CreateBookValidator>();

        return services;
    }
}
