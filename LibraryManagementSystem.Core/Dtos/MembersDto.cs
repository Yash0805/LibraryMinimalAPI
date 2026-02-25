namespace LibraryManagementSystem.Core.Dtos;

public sealed class MembersDto(int MemberId, string MemberName, string MemberType)
{
    public int MemberId { get; } = MemberId;
    public string MemberName { get; } = MemberName;
    public string MemberType { get; } = MemberType;
}