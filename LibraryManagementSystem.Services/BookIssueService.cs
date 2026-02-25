using LibraryManagementSystem.Core.Dtos;
using LibraryManagementSystem.Persistence;

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
        IReadOnlyList<BookIssueDto> BookIssues = _dbContext.BookIssue
            .Select
            (bi => new BookIssueDto
            (
                bi.IssueId,
                bi.MemberId,
                bi.BookId,
                bi.IssueDate,
                bi.ReturnDate,
                bi.RenewCount,
                bi.RenewDate,
                bi.Status
            ))
            .ToList();
        return BookIssues;
    }

    public BookIssueDto? GetBookIssueById(int IssueId)
    {
        var BookIssue = _dbContext.BookIssue.FirstOrDefault(bi => bi.IssueId == IssueId);
        if (BookIssue is null) return null;
        return new BookIssueDto(
            BookIssue.IssueId,
            BookIssue.MemberId,
            BookIssue.BookId,
            BookIssue.IssueDate,
            BookIssue.ReturnDate,
            BookIssue.RenewCount,
            BookIssue.RenewDate,
            BookIssue.Status
        );
    }
}