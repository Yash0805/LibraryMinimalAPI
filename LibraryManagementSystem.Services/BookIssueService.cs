using LibraryManagementSystem.Core.Dtos;
using LibraryManagementSystem.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace LibraryManagementSystem.Services;

public sealed class BookIssueService
{
    private readonly AppDbContext _dbContext;

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
}