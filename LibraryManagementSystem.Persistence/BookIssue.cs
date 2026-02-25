using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Persistence;

public sealed class BookIssue
{
    [Key] public int IssueId { get; set; }

    public required int MemberId { get; set; }
    public required int BookId { get; set; }
    public required DateOnly IssueDate { get; set; }
    public required DateOnly ReturnDate { get; set; }
    public int RenewCount { get; set; }
    public DateOnly? RenewDate { get; set; }
    public string Status { get; set; }
}