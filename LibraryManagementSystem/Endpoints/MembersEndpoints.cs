using LibraryManagementSystem.Core.Dtos;
using LibraryManagementSystem.Core.Request;
using LibraryManagementSystem.Persistence;
using LibraryManagementSystem.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LibraryManagementSystem.Web.Endpoints;

public static class MembersEndpoints
{
    public static IEndpointRouteBuilder MapMembersEndpoints(this IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);
        endpoints.MapGet("Members", GetMembers);
        endpoints.MapGet("Members/{MemberId:int}", GetMembersById);
        endpoints.MapPost("Members", CreateMemberRequest);
        endpoints.MapPut("Members/{MemberId:int}", UpdateMemberRequest);
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

        var validTypes = new[] { "Premium", "Regular" };
        if (!validTypes.Contains(request.MemberType, StringComparer.OrdinalIgnoreCase))
            return TypedResults.BadRequest("MemberType must be either 'Premium' or 'Regular'.");

        var result = membersService.CreateMemberRequest(request);
        return result is null
            ? TypedResults.Problem("There was some problem. See log for more details.")
            : TypedResults.Ok(result);
    }
    private static IResult UpdateMemberRequest(MembersService membersService, CreateMemberRequest request, int MemberId)
    {
        if (string.IsNullOrWhiteSpace(request.MemberName))
            return TypedResults.BadRequest("MemberName is required.");

        if (string.IsNullOrWhiteSpace(request.MemberType))
            return TypedResults.BadRequest("MemberType is required.");

        var validTypes = new[] { "Premium", "Regular" };
        if (!validTypes.Contains(request.MemberType, StringComparer.OrdinalIgnoreCase))
            return TypedResults.BadRequest("MemberType must be either 'Premium' or 'Regular'.");

        var result = membersService.UpdateMemberRequest(request,MemberId);
        return result is null
            ? TypedResults.Problem("There was some problem. See log for more details.")
            : TypedResults.Ok(result);
    }
}