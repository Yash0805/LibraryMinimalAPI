using LibraryManagementSystem.Core.Dtos;
using LibraryManagementSystem.Core.Request;
using LibraryManagementSystem.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LibraryManagementSystem.Services;

public sealed class BookIssueService
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<BookIssueService> _logger;

    public BookIssueService(AppDbContext dbContext, ILogger<BookIssueService> logger)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public IEnumerable<BookIssueDto> GetBookIssueList(string? MemberName = null)
    {
        IQueryable<BookIssue> query = _dbContext.BookIssue.AsQueryable();
        if (!string.IsNullOrWhiteSpace(MemberName))
        {
            query = query.Where(bi => bi.Member.MemberName.Contains(MemberName));
        }

        IReadOnlyList<BookIssueDto> bookIssues = query
            .Include(b => b.Book)
            .Include(m => m.Member)
            .Select
            (bi => new BookIssueDto
            (
                bi.IssueId,
                bi.Member.MemberName,
                bi.Member.MemberType,
                bi.Book.BookName,
                bi.IssueDate,
                bi.ReturnDate,
                bi.RenewCount,
                bi.RenewDate,
                bi.Status
            ))
            .ToList();
        return bookIssues;
    }

    public BookIssueDto? GetBookIssueById(int IssueId)
    {
        BookIssue? BookIssue = _dbContext.BookIssue
            .Include(b => b.Book)
            .Include(m => m.Member)
            .FirstOrDefault(bi => bi.IssueId == IssueId);
        if (BookIssue is null)
        {
            return null;
        }

        return new BookIssueDto(
            BookIssue.IssueId,
            BookIssue.Member.MemberName,
            BookIssue.Member.MemberType,
            BookIssue.Book.BookName,
            BookIssue.IssueDate,
            BookIssue.ReturnDate,
            BookIssue.RenewCount,
            BookIssue.RenewDate,
            BookIssue.Status
        );
    }

    public IEnumerable<BookIssueDto> GetBookByIssueDate(DateOnly? IssueDate)
    {
        IQueryable<BookIssue> query = _dbContext.BookIssue.AsQueryable();
        if (IssueDate.HasValue)
        {
            query = query.Where(bi => bi.IssueDate == IssueDate.Value);
        }

        IReadOnlyList<BookIssueDto> BookIssue = query
            .Include(b => b.Book)
            .Include(m => m.Member)
            .Select
            (bi => new BookIssueDto
            (
                bi.IssueId,
                bi.Member.MemberName,
                bi.Member.MemberType,
                bi.Book.BookName,
                bi.IssueDate,
                bi.ReturnDate,
                bi.RenewCount,
                bi.RenewDate,
                bi.Status
            ))
            .ToList();
        return BookIssue;
    }

    public IEnumerable<BookIssueDto> GetBookByReturnDate(DateOnly? ReturnDate)
    {
        IQueryable<BookIssue> query = _dbContext.BookIssue.AsQueryable();
        if (ReturnDate.HasValue)
        {
            query = query.Where(bi => bi.ReturnDate == ReturnDate.Value);
        }

        IReadOnlyList<BookIssueDto> BookIssue = query
            .Include(b => b.Book)
            .Include(m => m.Member)
            .Select
            (bi => new BookIssueDto
            (
                bi.IssueId,
                bi.Member.MemberName,
                bi.Member.MemberType,
                bi.Book.BookName,
                bi.IssueDate,
                bi.ReturnDate,
                bi.RenewCount,
                bi.RenewDate,
                bi.Status
            ))
            .ToList();
        return BookIssue;
    }

    public BookIssueDto BookView(BookIssue bookIssue)
    {
        return new BookIssueDto(
            bookIssue.IssueId,
            bookIssue.Member.MemberName,
            bookIssue.Member.MemberType,
            bookIssue.Book.BookName,
            bookIssue.IssueDate,
            bookIssue.ReturnDate,
            bookIssue.RenewCount,
            bookIssue.RenewDate,
            bookIssue.Status
        );
    }

    public BookIssueDto? CreateBookIssueRequest(CreateBookIssueRequest request)
    {
        try
        {
            Members? members = _dbContext.Members
                .FirstOrDefault(m => m.MemberId == request.MemberId);
            if (members == null)
            {
                throw new ConflictException($"Member with ID {request.MemberId} not found.", request.MemberId);
            }

            Books? book = _dbContext.Books
                .FirstOrDefault(b => b.BookId == request.BookId);
            if (book == null)
            {
                throw new ConflictException($"Book with ID {request.BookId} does not exist.", request.BookId);
            }

            bool bookAlreadyIssued = _dbContext.BookIssue
                .Any(b => b.BookId == request.BookId && b.Status != "Returned");
            if (bookAlreadyIssued)
            {
                throw new Exception("This book is already issued to another member.");
            }

            int issuedBooksCount = _dbContext.BookIssue
                .Count(b => b.MemberId == request.MemberId && b.Status != "Returned");
            if (members.MemberType == "Premium" && issuedBooksCount >= 4)
            {
                throw new Exception("Premium members can issue maximum 4 books at a time.");
            }

            if (members.MemberType == "Regular" && issuedBooksCount >= 2)
            {
                throw new Exception("Regular members can issue maximum 2 books at a time.");
            }

            BookIssue bookIssue = new()
            {
                MemberId = request.MemberId,
                BookId = request.BookId,
                IssueDate = DateOnly.FromDateTime(DateTime.Today),
                ReturnDate = DateOnly.FromDateTime(DateTime.Today).AddDays(15)
            };
            _dbContext.BookIssue.Add(bookIssue);
            _dbContext.SaveChanges();

            BookIssue? createdIssue = _dbContext.BookIssue
                .Include(b => b.Book)
                .Include(m => m.Member)
                .FirstOrDefault(b => b.IssueId == bookIssue.IssueId);

            if (createdIssue == null)
            {
                return null;
            }

            return BookView(bookIssue);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error while creating Books Issue for member id {MemberId}",
                request.MemberId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Unexpected error while creating Books Issue for Member Id {MemberId} ",
                request.MemberId);
        }

        return null;
    }

    public BookIssueDto? PatchBookIssueRequest(PatchBookIssueRequest request, int IssueId)
    {
        try
        {
            BookIssue? bookIssue = _dbContext.BookIssue
                .Include(b => b.Book)
                .Include(m => m.Member)
                .FirstOrDefault(b => b.IssueId == IssueId);

            if (bookIssue is null)
            {
                throw new Exception($"Book issue with id {IssueId} not found");
            }

            bookIssue.Status = "Returned";

            _dbContext.SaveChanges();

            return BookView(bookIssue);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error while returning book for IssueId {IssueId}", IssueId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while returning book for IssueId {IssueId}", IssueId);
        }

        return null;
    }

    public BookIssueDto PatchRenewedBookIssueRequest(PatchRenewedBookIssueRequest request, int IssueId)
    {
        try
        {
            BookIssue? bookIssue = _dbContext.BookIssue
                .Include(b => b.Book)
                .Include(m => m.Member)
                .FirstOrDefault(b => b.IssueId == IssueId);
            if (bookIssue is null)
            {
                throw new Exception($"Book issue with id {IssueId} not found");
            }

            if (bookIssue.RenewCount >= 1)
            {
                throw new Exception("Renewal limit reached. Book can only be renewed 1 time.");
            }

            bookIssue.ReturnDate = DateOnly.FromDateTime(DateTime.Today).AddDays(15);
            bookIssue.RenewCount += 1;
            bookIssue.RenewDate = DateOnly.FromDateTime(DateTime.Today);
            bookIssue.Status = "Renewed";

            _dbContext.SaveChanges();

            return BookView(bookIssue);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error while creating book for IssueId {IssueId}", IssueId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while returning book for IssueId {IssueId}", IssueId);
        }

        return null;
    }

    public BookIssueDto DeleteBookIssue(int issueId)
    {
        try
        {
            BookIssue? bookIssue = _dbContext.BookIssue
                .Include(b => b.Book)
                .Include(m => m.Member)
                .FirstOrDefault(b => b.IssueId == issueId);

            if (bookIssue is null)
            {
                throw new ConflictException($"BookIssue with ID {issueId} not found.");
            }

            if (bookIssue.Status != "Returned")
            {
                throw new Exception("Cannot delete. Book is not returned yet.");
            }

            _dbContext.BookIssue.Remove(bookIssue);
            _dbContext.SaveChanges();

            return new BookIssueDto(
                bookIssue.IssueId,
                bookIssue.Member.MemberName,
                bookIssue.Member.MemberType,
                bookIssue.Book.BookName,
                bookIssue.IssueDate,
                bookIssue.ReturnDate,
                bookIssue.RenewCount,
                bookIssue.RenewDate,
                bookIssue.Status
            );
        }
        catch (ConflictException ex)
        {
            _logger.LogError(ex, "BookIssue not found with ID {IssueId}", issueId);
            throw;
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error while deleting BookIssue with ID {IssueId}", issueId);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while deleting BookIssue with ID {IssueId}", issueId);
            throw;
        }
    }

    public BookIssueDto? UpdateBookIssue(int issueId, CreateBookIssueRequest request)
    {
        try
        {
            BookIssue? bookIssue = _dbContext.BookIssue
                .Include(b => b.Book)
                .Include(m => m.Member)
                .FirstOrDefault(b => b.IssueId == issueId);

            if (bookIssue is null)
            {
                _logger.LogWarning("BookIssue not found with ID {IssueId}", issueId);
                return null;
            }

            bool memberExists = _dbContext.Members.Any(m => m.MemberId == request.MemberId);
            if (!memberExists)
            {
                throw new Exception($"Member with ID {request.MemberId} not found");
            }

            bool bookExists = _dbContext.Books.Any(b => b.BookId == request.BookId);
            if (!bookExists)
            {
                throw new Exception($"Book with ID {request.BookId} not found");
            }

            bookIssue.MemberId = request.MemberId;
            bookIssue.BookId = request.BookId;
            bookIssue.IssueDate = request.IssueDate;
            bookIssue.ReturnDate = request.ReturnDate;

            _dbContext.SaveChanges();

            return BookView(bookIssue);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error while updating BookIssue with ID {IssueId}", issueId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while updating BookIssue with ID {IssueId}", issueId);
        }

        return null;
    }
}
