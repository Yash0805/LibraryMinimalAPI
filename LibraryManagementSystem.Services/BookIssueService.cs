using LibraryManagementSystem.Core.Dtos;
using LibraryManagementSystem.Core.Request;
using LibraryManagementSystem.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace LibraryManagementSystem.Services;

public sealed class BookIssueService
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<BookIssueService> _logger;

    public BookIssueService(AppDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public IEnumerable<BookIssueDto> GetBookIssueList(string? MemberName = null)
    {
        IQueryable<BookIssue> query = _dbContext.BookIssue.AsQueryable();
        if(!string.IsNullOrEmpty(MemberName))
        {
            query = query.Where(bi => bi.Member.MemberName.Contains(MemberName));
        }
        IReadOnlyList<BookIssueDto> bookIssues = query
            .Include(b=>b.Book)
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
        var BookIssue = _dbContext.BookIssue
            .Include(b => b.Book)
            .Include(m => m.Member)
            .FirstOrDefault(bi => bi.IssueId == IssueId);
        if (BookIssue is null) return null;
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
    public BookIssueDto? CreateBookIssueRequest(CreateBookIssueRequest request)
    {
        try
        {
            var bookIssue = new BookIssue
            {
                MemberId = request.MemberId,
                BookId = request.BookId,
                IssueDate = request.IssueDate,
                ReturnDate = request.ReturnDate,
                RenewCount = request.RenewCount,
                RenewDate = request.RenewDate,  
                Status = request.Status
            };
            _dbContext.BookIssue.Add(bookIssue);
            _dbContext.SaveChanges();

      
            var BookIssueDto = new BookIssueDto(
                bookIssue.IssueId,
                 _dbContext.Members
                    .Where(m => m.MemberId == bookIssue.MemberId)
                    .Select(m => m.MemberName)
                    .FirstOrDefault() ?? string.Empty,
                 _dbContext.Members
                    .Where(m => m.MemberId == bookIssue.MemberId)
                    .Select(m => m.MemberType)
                    .FirstOrDefault() ?? string.Empty,
                 _dbContext.Books
                    .Where(b => b.BookId == bookIssue.BookId)
                    .Select(b=> b.BookName)
                    .FirstOrDefault() ?? string.Empty,
                bookIssue.IssueDate,
                bookIssue.ReturnDate,
                bookIssue.RenewCount,
                bookIssue.RenewDate,
                bookIssue.Status
                );
            return BookIssueDto;
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
            var bookIssue = _dbContext.BookIssue.Find(IssueId);
            if (bookIssue is null) throw new Exception($"book issue with id {IssueId} not found");
            bookIssue.Status = request.Status;
            _dbContext.SaveChanges();


            var BookIssueDto = new BookIssueDto(
                bookIssue.IssueId,
                 _dbContext.Members
                    .Where(m => m.MemberId == bookIssue.MemberId)
                    .Select(m => m.MemberName)
                    .FirstOrDefault() ?? string.Empty,
                 _dbContext.Members
                    .Where(m => m.MemberId == bookIssue.MemberId)
                    .Select(m => m.MemberType)
                    .FirstOrDefault() ?? string.Empty,
                 _dbContext.Books
                    .Where(b => b.BookId == bookIssue.BookId)
                    .Select(b => b.BookName)
                    .FirstOrDefault() ?? string.Empty,
                bookIssue.IssueDate,
                bookIssue.ReturnDate,
                bookIssue.RenewCount,
                bookIssue.RenewDate,
                bookIssue.Status
                );
            return BookIssueDto;
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error while patching Issue Book with id {IssueId}", IssueId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while patching Issue Book with id  {IssueId}", IssueId);
        }
        return null;
    }
}