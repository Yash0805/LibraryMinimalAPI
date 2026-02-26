namespace LibraryManagementSystem.Core.Dtos;

public sealed class BooksDto(
    int BookId,
    string BookName,
    string Publisher,
    string Author,
    decimal Price,
    string CategoryName)
{
    public int BookId { get; } = BookId;
    public string BookName { get; } = BookName;
    public string Publisher { get; } = Publisher;
    public string Author { get; } = Author;
    public decimal Price { get; } = Price;
    public string CategoryName { get; } = CategoryName;
}