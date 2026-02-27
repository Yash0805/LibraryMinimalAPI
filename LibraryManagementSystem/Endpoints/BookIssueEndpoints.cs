using LibraryManagementSystem.Core.Dtos;
using LibraryManagementSystem.Core.Request;
using LibraryManagementSystem.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LibraryManagementSystem.Web.Endpoints;

public static class BookIssueEndpoints
{
    public static IEndpointRouteBuilder MapBookIssueEndpoints(this IEndpointRouteBuilder endpoint)
    {
        ArgumentNullException.ThrowIfNull(endpoint);
        endpoint.MapGet("BookIssue", GetBookIssueList);
        endpoint.MapGet("BookIssue/{IssueId:int}", GetBookIssueById);
        endpoint.MapPost("BookIssue", CreateBookIssueRequest);
        endpoint.MapPatch("BookIssue/{IssueId:int}", PatchBookIssueRequest);
        return endpoint;
    }

    private static Ok<IEnumerable<BookIssueDto>> GetBookIssueList(BookIssueService bookIssueService, string? MemberName)
    {
        var bookIssues = bookIssueService.GetBookIssueList(MemberName);
        return TypedResults.Ok(bookIssues);
    }

    public static IResult GetBookIssueById(BookIssueService bookIssueservice, int IssueId)
    {
        var bookIssues = bookIssueservice.GetBookIssueById(IssueId);
        return bookIssues is null ? TypedResults.NotFound() : TypedResults.Ok(bookIssues);
    }

    private static IResult CreateBookIssueRequest(BookIssueService bookIssueservice, CreateBookIssueRequest request)
    {
        var result = bookIssueservice.CreateBookIssueRequest(request);
        return result is null
           ? TypedResults.Problem("There was some problem. See log for more details.")
           : TypedResults.Ok(result);
    }
    private static IResult PatchBookIssueRequest(BookIssueService bookIssueservice, PatchBookIssueRequest request, int IssueId )
    {
        var result = bookIssueservice.PatchBookIssueRequest(request,IssueId);
        return result is null
        ? TypedResults.Problem("There was some problem. See log for more details.")
        : TypedResults.Ok(result);
    }
}