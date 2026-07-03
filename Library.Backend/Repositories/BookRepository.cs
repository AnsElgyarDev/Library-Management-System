using System.Text.Json;
using Library.Core.Model;

namespace Library.Core.Repository;

class BookRepository : IBookRepository 
{
    public string FilePath = @"D:\Desktop\Library Management System\Library.Backend\books.Json";
    public List<Book> GetAllBooks()
    {
        if(!File.Exists(FilePath))
        {
            return new List<Book>();
        }

        string StringFile = File.ReadAllText(FilePath);
        return JsonSerializer.Deserialize<List<Book>>(StringFile) ?? new List<Book>();
    }

    public void SaveBooks(List<Book> books)
    {
        string JsonString = JsonSerializer.Serialize(books, new JsonSerializerOptions{ WriteIndented = true });

        File.WriteAllText(FilePath, JsonString);
    }
}