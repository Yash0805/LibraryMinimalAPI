namespace LibraryManagementSystem.Core.Request;

public class CreateMemberRequest
{
    public required string MemberName { get; set; }
    public required string MemberType { get; set; }
}
