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
            .Include(c => c.Category)
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
        Books? Book = _dbContext.Books
            .Include(c => c.Category)
            .FirstOrDefault(b => b.BookId == BookId);
        if (Book is null)
        {
            return null;
        }

        return new BooksDto(
            Book.BookId,
            Book.BookName,
            Book.Publisher,
            Book.Author,
            Book.Price,
            Book.Category.CategoryName
        );
    }

    public BooksDto? CreateBooksRequest(CreateBooksRequest request)
    {
        try
        {
            Books Book = new()
            {
                BookName = request.BookName,
                Publisher = request.Publisher,
                Author = request.Author,
                Price = request.Price,
                CategoryId = request.CategoryId
            };
            _dbContext.Books.Add(Book);
            _dbContext.SaveChanges();

            BooksDto BooksDto = new(
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

    public BooksDto DeleteBooksRequest(int bookId)
    {
        try
        {
            Books? book = _dbContext.Books
                .Include(b => b.Category)
                .FirstOrDefault(b => b.BookId == bookId);

            if (book is null)
            {
                throw new ConflictException($"Book with ID {bookId} not found.");
            }

            _dbContext.Books.Remove(book);
            _dbContext.SaveChanges();

            return new BooksDto(
                book.BookId,
                book.BookName,
                book.Publisher,
                book.Author,
                book.Price,
                book.Category?.CategoryName ?? string.Empty
            );
        }
        catch (ConflictException ex)
        {
            _logger.LogError(ex, "Book not found with ID {BookId}", bookId);
            throw;
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error while deleting book with ID {BookId}", bookId);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while deleting book with ID {BookId}", bookId);
            throw;
        }
    }

    public BooksDto? UpdateBook(int bookId, CreateBooksRequest request)
    {
        try
        {
            Books? book = _dbContext.Books
                .Include(b => b.Category)
                .FirstOrDefault(b => b.BookId == bookId);

            if (book is null)
            {
                _logger.LogWarning("Book not found with ID {BookId}", bookId);
                return null;
            }

            book.BookName = request.BookName;
            book.Publisher = request.Publisher;
            book.Author = request.Author;
            book.Price = request.Price;
            book.CategoryId = request.CategoryId;

            _dbContext.SaveChanges();

            return new BooksDto(
                book.BookId,
                book.BookName,
                book.Publisher,
                book.Author,
                book.Price,
                book.Category?.CategoryName ?? string.Empty
            );
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error while updating book with ID {BookId}", bookId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while updating book with ID {BookId}", bookId);
        }

        return null;
    }
}
