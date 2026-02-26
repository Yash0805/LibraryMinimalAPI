using LibraryManagementSystem.Core.Dtos;
using LibraryManagementSystem.Core.Request;
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
        endpoints.MapPost("Members", CreateMemberRequest);
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

    private static IResult CreateMemberRequest(MembersService membersService, CreateMemberRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.MemberName))
            return TypedResults.BadRequest("MemberName is required.");
        if (string.IsNullOrWhiteSpace(request.MemberType))
            return TypedResults.BadRequest("MemberType is required.");
        var result = membersService.CreateMemberRequest(request);
        return result is null
            ? TypedResults.Problem("There was some problem. See log for more details.")
            : TypedResults.Ok(result);
    }
}