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
        endpoints.MapGet("Members/{MemberId:int}", GetMembersById);
        endpoints.MapPost("Members", CreateMemberRequest);
        endpoints.MapPut("Members/{MemberId:int}", UpdateMemberRequest);
        endpoints.MapDelete("Members/{MemberId:int}", DeleteMember);
        return endpoints;
    }

    private static Ok<IEnumerable<MembersDto>> GetMembers(MembersService memberService)
    {
        IEnumerable<MembersDto> Members = memberService.GetMembersList();
        return TypedResults.Ok(Members);
    }

    private static IResult GetMembersById(MembersService membersService, int MemberId)
    {
        MembersDto? Members = membersService.GetMembersById(MemberId);
        return Members is null ? TypedResults.NotFound() : TypedResults.Ok(Members);
    }

    private static IResult CreateMemberRequest(MembersService membersService, CreateMemberRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.MemberName))
        {
            return TypedResults.BadRequest("MemberName is required.");
        }

        if (string.IsNullOrWhiteSpace(request.MemberType))
        {
            return TypedResults.BadRequest("MemberType is required.");
        }

        string[] validTypes = new[] { "Premium", "Regular" };
        if (!validTypes.Contains(request.MemberType, StringComparer.OrdinalIgnoreCase))
        {
            return TypedResults.BadRequest("MemberType must be either 'Premium' or 'Regular'.");
        }

        MembersDto? result = membersService.CreateMemberRequest(request);
        return result is null
            ? TypedResults.Problem("There was some problem. See log for more details.")
            : TypedResults.Ok(result);
    }

    private static IResult UpdateMemberRequest(MembersService membersService, CreateMemberRequest request, int MemberId)
    {
        if (string.IsNullOrWhiteSpace(request.MemberName))
        {
            return TypedResults.BadRequest("MemberName is required.");
        }

        if (string.IsNullOrWhiteSpace(request.MemberType))
        {
            return TypedResults.BadRequest("MemberType is required.");
        }

        string[] validTypes = new[] { "Premium", "Regular" };
        if (!validTypes.Contains(request.MemberType, StringComparer.OrdinalIgnoreCase))
        {
            return TypedResults.BadRequest("MemberType must be either 'Premium' or 'Regular'.");
        }

        MembersDto? result = membersService.UpdateMemberRequest(request, MemberId);
        return result is null
            ? TypedResults.Problem("There was some problem. See log for more details.")
            : TypedResults.Ok(result);
    }

    private static IResult DeleteMember(MembersService membersService, int MemberId)
    {
        try
        {
            MembersDto members = membersService.DeleteMemberRequest(MemberId);
            return TypedResults.Ok(members);
        }
        catch (KeyNotFoundException e)
        {
            return TypedResults.NotFound();
        }
    }
}
