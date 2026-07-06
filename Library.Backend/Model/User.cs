namespace Library.Core.Model;
public class User
{
    public int Id { get; set; }
    public const int BooksLimit = 3;
    public string Name { get; set; } = null!;
    public Role Role { get; set; }
    public int BorrowedBooks { get; set; }
    public List<Book> Books { get; set; } = null!;
    public override string ToString() => $"USer {Id}\nName: {Name}";
}
