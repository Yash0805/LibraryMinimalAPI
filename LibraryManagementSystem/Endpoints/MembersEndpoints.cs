using LibraryManagementSystem.Core.Dtos;
using LibraryManagementSystem.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LibraryManagementSystem.Web.Endpoints;

public static class MembersEndpoints
{
    public static IEndpointRouteBuilder MapMembersEndpoints(this IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);
        endpoints.MapGet("Members", GetMembers);
        endpoints.MapGet("Members/{MemberId}", GetMembersById);
        return endpoints;
    }

    private static Ok<IEnumerable<MembersDto>> GetMembers(MembersService memberService)
    {
        var Members = memberService.GetMembersList();
        return TypedResults.Ok(Members);
    }

    private static IResult GetMembersById(MembersService membersService, int MemberId)
    {
        var Members = membersService.GetMembersById(MemberId);
        return Members is null ? TypedResults.NotFound() : TypedResults.Ok(Members);
    }
}