namespace LibraryManagementSystem.Core.Request;

public class CreateBookIssueRequest
{
    public int MemberId { get; set; }
    public int BookId { get; set; }
    public DateOnly IssueDate { get; set; }
    public DateOnly ReturnDate { get; set; }
}
