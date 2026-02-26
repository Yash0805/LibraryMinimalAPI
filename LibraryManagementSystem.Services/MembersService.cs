using LibraryManagementSystem.Core.Dtos;
using LibraryManagementSystem.Core.Request;
using LibraryManagementSystem.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LibraryManagementSystem.Services;

public sealed class MembersService
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<MembersService> _logger;

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

    public MembersDto? CreateMemberRequest(CreateMemberRequest request )
    {
        try
        {
            var Members = new Members
            {
                MemberName = request.MemberName,
                MemberType = request.MemberType
            };
            _dbContext.Members.Add(Members);
            _dbContext.SaveChanges();

            var MembersDto = new MembersDto(
                Members.MemberId,
                Members.MemberName,
                Members.MemberType);
            return MembersDto;
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex,
               "Database error while creating Member for Member  name {MemberName} ",
                request.MemberName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Unexpected error while creating Member for Member  name {MemberName} ",
               request.MemberName);
        }
        return null;
    }
}