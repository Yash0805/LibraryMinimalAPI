using LibraryManagementSystem.Core.Dtos;
using LibraryManagementSystem.Core.Request;
using LibraryManagementSystem.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LibraryManagementSystem.Services;

public sealed class BooksService
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<BooksService> _logger;

    public BooksService(AppDbContext dbContext, ILogger<BooksService> logger)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // ✅ GET LIST
    public IEnumerable<BooksDto> GetBooksList(string? BookName = null)
    {
        IQueryable<Books> query = _dbContext.Books.Include(b => b.Category);

        if (!string.IsNullOrEmpty(BookName))
        {
            query = query.Where(b => b.BookName.Contains(BookName));
        }

        return query.Select(b => new BooksDto(
            b.BookId,
            b.BookName,
            b.Publisher,
            b.Author,
            b.Price,
            b.CategoryId,
            b.Category.CategoryName
        )).ToList();
    }

    // ✅ GET BY ID
    public BooksDto? GetBooksById(int BookId)
    {
        var book = _dbContext.Books
            .Include(b => b.Category)
            .FirstOrDefault(b => b.BookId == BookId);

        if (book is null) return null;

        return new BooksDto(
            book.BookId,
            book.BookName,
            book.Publisher,
            book.Author,
            book.Price,
            book.CategoryId,
            book.Category?.CategoryName ?? string.Empty
        );
    }

    public BooksDto? CreateBooksRequest(CreateBooksRequest request)
    {
        try
        {
            var book = new Books
            {
                BookName = request.BookName,
                Publisher = request.Publisher,
                Author = request.Author,
                Price = request.Price,
                CategoryId = request.CategoryId
            };

            _dbContext.Books.Add(book);
            _dbContext.SaveChanges();

            var category = _dbContext.Category
                .FirstOrDefault(c => c.CategoryId == book.CategoryId);

            return new BooksDto(
                book.BookId,
                book.BookName,
                book.Publisher,
                book.Author,
                book.Price,
                book.CategoryId,
                category?.CategoryName ?? string.Empty
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating book");
            return null;
        }
    }

    public BooksDto DeleteBooksRequest(int bookId)
    {
        var book = _dbContext.Books
            .Include(b => b.Category)
            .FirstOrDefault(b => b.BookId == bookId);

        if (book is null)
            throw new Exception("Book not found");

        _dbContext.Books.Remove(book);
        _dbContext.SaveChanges();

        return new BooksDto(
            book.BookId,
            book.BookName,
            book.Publisher,
            book.Author,
            book.Price,
            book.CategoryId,
            book.Category?.CategoryName ?? string.Empty
        );
    }

    public BooksDto? UpdateBook(int bookId, CreateBooksRequest request)
    {
        try
        {
            var book = _dbContext.Books
                .Include(b => b.Category)
                .FirstOrDefault(b => b.BookId == bookId);

            if (book is null)
                return null;

            book.BookName = request.BookName;
            book.Publisher = request.Publisher;
            book.Author = request.Author;
            book.Price = request.Price;
            book.CategoryId = request.CategoryId;

            _dbContext.SaveChanges();

            _dbContext.Entry(book).Reference(b => b.Category).Load();

            return new BooksDto(
                book.BookId,
                book.BookName,
                book.Publisher,
                book.Author,
                book.Price,
                book.CategoryId,
                book.Category?.CategoryName ?? string.Empty
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating book");
            return null;
        }
    }
}
