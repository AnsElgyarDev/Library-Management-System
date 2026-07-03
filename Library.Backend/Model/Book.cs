namespace Library.Core.Model;

public class Book
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty; 
    public string Author { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string SerialNumber {get; set;} = string.Empty;
    public bool IsAvailable { get; set; } = true; 
    public DateTime PublishDate { get; set; } = DateTime.UtcNow; 
}