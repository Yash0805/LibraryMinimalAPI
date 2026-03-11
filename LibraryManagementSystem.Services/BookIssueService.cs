using LibraryManagementSystem.Core.Dtos;
using LibraryManagementSystem.Core.Request;
using LibraryManagementSystem.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Net.NetworkInformation;

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
    public BookIssueDto BookView(BookIssue bookIssue)
    {
        var member = _dbContext.Members
            .Where(m => m.MemberId == bookIssue.MemberId)
            .Select(m => new { m.MemberName, m.MemberType })
            .FirstOrDefault();

        var bookName = _dbContext.Books
            .Where(b => b.BookId == bookIssue.BookId)
            .Select(b => b.BookName)
            .FirstOrDefault();

        return new BookIssueDto(
            bookIssue.IssueId,
            member?.MemberName ?? string.Empty,
            member?.MemberType ?? string.Empty,
            bookName ?? string.Empty,
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
            var bookIssue = new BookIssue
            {
                MemberId = request.MemberId,
                BookId = request.BookId,
                IssueDate = DateOnly.FromDateTime(DateTime.Today),
                ReturnDate = DateOnly.FromDateTime(DateTime.Today).AddDays(15),
            };
            _dbContext.BookIssue.Add(bookIssue);
            _dbContext.SaveChanges();
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
        var bookIssue = _dbContext.BookIssue.Find(IssueId);
        if (bookIssue is null)
            throw new Exception($"Book issue with id {IssueId} not found");
        bookIssue.Status = "Returned";
        _dbContext.SaveChanges();
        return BookView(bookIssue);
        //try
        //{
        //    var bookIssue = _dbContext.BookIssue.Find(IssueId);
        //    if (bookIssue is null) throw new Exception($"book issue with id {IssueId} not found");
        //    bookIssue.Status = "Returned";
        //    _dbContext.SaveChanges();
        //    return BookView(bookIssue);
        //}
        //catch (DbUpdateException ex)
        //{
        //    _logger.LogError(ex, "Database error while patching Issue Book with id {IssueId}", IssueId);
        //}
        //catch (Exception ex)
        //{
        //    _logger.LogError(ex, "Unexpected error while patching Issue Book with id  {IssueId}", IssueId);
        //}
        //return null;
    }

    public BookIssueDto PatchRenewedBookIssueRequest(PatchRenewedBookIssueRequest request, int IssueId)
    {
        var bookIssue = _dbContext.BookIssue.Find(IssueId);

        if (bookIssue is null)
            throw new Exception($"Book issue with id {IssueId} not found");

        if (bookIssue.RenewCount >= 1)
            throw new Exception("Renewal limit reached. Book can only be renewed 1 time.");

        bookIssue.ReturnDate = DateOnly.FromDateTime(DateTime.Today).AddDays(15);
        bookIssue.RenewCount += 1;
        bookIssue.RenewDate = DateOnly.FromDateTime(DateTime.Today);
        bookIssue.Status = "Renewed";

        _dbContext.SaveChanges();

        return BookView(bookIssue);
    }
}