using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Persistence;

public sealed class Members
{
    [Key] public int MemberId { get; set; }

    public required string MemberName { get; set; }
    public required string MemberType { get; set; }
}