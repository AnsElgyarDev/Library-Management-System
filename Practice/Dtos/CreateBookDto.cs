namespace Practice.DTO;

// a standard template to recieve Data To Users
public record CreateBookDto(string Name, string Author, decimal Price, string serialNumber);