using LibraryManagementSystem.Core.Dtos;
using LibraryManagementSystem.Persistence;

namespace LibraryManagementSystem.Services;

public sealed class MembersService
{
    private readonly AppDbContext _dbContext;

    public MembersService(AppDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public IEnumerable<MembersDto> GetMembersList()
    {
        IReadOnlyList<MembersDto> Members = _dbContext.Members
            .Select
            (m => new MembersDto
            (
                m.MemberId,
                m.MemberName,
                m.MemberType
            ))
            .ToList();
        return Members;
    }

    public MembersDto? GetMembersById(int MemberId)
    {
        var Members = _dbContext.Members.FirstOrDefault(m => m.MemberId == MemberId);
        if (Members is null) return null;
        return new MembersDto(
            Members.MemberId,
            Members.MemberName,
            Members.MemberType);
    }
}