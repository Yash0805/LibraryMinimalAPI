using LibraryManagementSystem.Core.Dtos;
using LibraryManagementSystem.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace LibraryManagementSystem.Services;

public sealed class BooksService
{
    private readonly AppDbContext _dbContext;

    public BooksService(AppDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public IEnumerable<BooksDto> GetBooksList(string? BookName = null)
    {
        IQueryable<Books> query = _dbContext.Books.AsQueryable();
        if (!string.IsNullOrEmpty(BookName))
        {
            query = query.Where(b => b.BookName.Contains(BookName));
        }
        IReadOnlyList<BooksDto> Books = query
            .Include (c => c.Category)
            .Select
            (b => new BooksDto
            (
                b.BookId,
                b.BookName,
                b.Publisher,
                b.Author,
                b.Price,
                b.Category.CategoryName
            ))
            .ToList();
        return Books;
    }

    public BooksDto? GetBooksById(int BookId)
    {
        var Book = _dbContext.Books
            .Include(c => c.Category)
            .FirstOrDefault(b => b.BookId == BookId);
        if (Book is null) return null;
        return new BooksDto(
            Book.BookId,
            Book.BookName,
            Book.Publisher,
            Book.Author,
            Book.Price,
            Book.Category.CategoryName
        );

    }
}
