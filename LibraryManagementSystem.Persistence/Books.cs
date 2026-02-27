using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Persistence;

public sealed class Books
{
    [Key] public int BookId { get; set; }
    public required string BookName { get; set; }
    public required string Publisher { get; set; }
    public required string Author { get; set; }
    public required decimal Price { get; set; }
    public int CategoryId { get; set; }
    public  Category Category { get; set; }
    public IList<BookIssue> BookIssue { get; init; } = [];
}