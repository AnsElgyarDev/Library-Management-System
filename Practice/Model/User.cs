namespace Practice.Model;
public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public Role Role { get; set; }
    public List<Book> Books { get; set; } = null!;
    public override string ToString() => $"USer {Id}\nName: {Name}";
}

public enum Role
{
    Admin = 1,
    User
}