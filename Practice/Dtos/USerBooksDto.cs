using Practice.Model;
namespace Practice.DTO;

public record UserBooksDto(string Name, List<Book> books);