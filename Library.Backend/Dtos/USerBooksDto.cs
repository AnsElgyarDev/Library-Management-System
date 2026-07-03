using Practice.Model;
namespace Library.Core.DTO;

public record UserBooksDto(string Name, List<Book> books);