using LibraryManagementSystem.Core.Dtos;
using LibraryManagementSystem.Core.Request;
using LibraryManagementSystem.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Net;

namespace LibraryManagementSystem.Services;

public sealed class BooksService
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<BooksService> _logger;

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

    public BooksDto? CreateBooksRequest (CreateBooksRequest request)
    {
        try
        {
            var Book = new Books
            {
                BookName = request.BookName,
                Publisher = request.Publisher,
                Author = request.Author,
                Price = request.Price,
                CategoryId = request.CategoryId
            };
            _dbContext.Books.Add(Book);
            _dbContext.SaveChanges();

            var BooksDto = new BooksDto(
                Book.BookId,
                Book.BookName,
                Book.Publisher,
                Book.Author,
                Book.Price,
                 _dbContext.Category
                 .Where(c => c.CategoryId == Book.CategoryId)
                 .Select(c => c.CategoryName)
                 .FirstOrDefault() ?? string.Empty
                 );
            return BooksDto;
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error while creating Books for Books name {BookName}",
                request.BookName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Unexpected error while creating Books for Books name {BookName} ",
               request.BookName);
        }
        return null;
    }
}
