using LibraryManagementSystem.Core.Dtos;
using LibraryManagementSystem.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Services;

public sealed class BookIssueService
{
    private readonly AppDbContext _dbContext;

    public BookIssueService(AppDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public IEnumerable<BookIssueDto> GetBookIssueList()
    {
        IReadOnlyList<BookIssueDto> BookIssue = _dbContext.BookIssue
            .Include(b=>b.Book)
            .Include(m => m.Member)
            .Select
            (bi => new BookIssueDto
            (
                bi.IssueId,
                bi.Member.MemberName,
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
            BookIssue.Book.BookName,
            BookIssue.IssueDate,
            BookIssue.ReturnDate,
            BookIssue.RenewCount,
            BookIssue.RenewDate,
            BookIssue.Status
        );
    }
}