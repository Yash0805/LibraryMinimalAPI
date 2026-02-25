namespace LibraryManagementSystem.Core.Dtos;

public sealed class BookIssueDto(
    int IssueId,
    int MemberId,
    int BookId,
    DateOnly IssueDate,
    DateOnly ReturnDate,
    int RenewCount,
    DateOnly? RenewDate,
    string Status)
{
    public int IssueId { get; } = IssueId;
    public int MemberId { get; } = MemberId;
    public int BookId { get; } = BookId;
    public DateOnly IssueDate { get; } = IssueDate;
    public DateOnly ReturnDate { get; } = ReturnDate;
    public int RenewCount { get; } = RenewCount;
    public DateOnly? RenewDate { get; } = RenewDate;
    public string Status { get; } = Status;
}