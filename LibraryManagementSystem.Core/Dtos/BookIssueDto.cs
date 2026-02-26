namespace LibraryManagementSystem.Core.Dtos;

public sealed class BookIssueDto(
    int IssueId,
    string MemberName,
    string MemberType,
    string BookName,
    DateOnly IssueDate,
    DateOnly ReturnDate,
    int RenewCount,
    DateOnly? RenewDate,
    string Status)
{
    public int IssueId { get; } = IssueId;
    public string MemberName { get; } = MemberName;
    public string MemberType { get; } = MemberType;
    public string BookName { get; } = BookName;
    public DateOnly IssueDate { get; } = IssueDate;
    public DateOnly ReturnDate { get; } = ReturnDate;
    public int RenewCount { get; } = RenewCount;
    public DateOnly? RenewDate { get; } = RenewDate;
    public string Status { get; } = Status;
}