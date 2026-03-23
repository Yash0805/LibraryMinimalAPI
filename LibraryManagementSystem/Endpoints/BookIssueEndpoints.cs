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
        endpoint.MapGet("BookIssue/IssueDate/{IssueDate}", GetBookByIssueDate);
        endpoint.MapGet("BookIssue/ReturnDate/{ReturnDate}", GetBookByReturnDate);
        endpoint.MapPost("BookIssue", CreateBookIssueRequest);
        endpoint.MapPatch("BookIssue/{IssueId:int}", PatchBookIssueRequest);
        endpoint.MapPatch("BookIssue/Renewed/{IssueId:int}", PatchRenewedBookIssueRequest);
        endpoint.MapDelete("BookIssue/{issueId}", DeleteBookIssue);
        endpoint.MapPut("BookIssue/{issueId}", UpdateBookIssue);
        return endpoint;
    }

    private static Ok<IEnumerable<BookIssueDto>> GetBookIssueList(BookIssueService bookIssueService, string? MemberName)
    {
        IEnumerable<BookIssueDto> bookIssues = bookIssueService.GetBookIssueList(MemberName);
        return TypedResults.Ok(bookIssues);
    }

    public static IResult GetBookIssueById(BookIssueService bookIssueservice, int IssueId)
    {
        BookIssueDto? bookIssues = bookIssueservice.GetBookIssueById(IssueId);
        return bookIssues is null ? TypedResults.NotFound() : TypedResults.Ok(bookIssues);
    }

    public static IResult GetBookByIssueDate(BookIssueService bookIssueService, DateOnly IssueDate)
    {
        IEnumerable<BookIssueDto> BookIssue = bookIssueService.GetBookByIssueDate(IssueDate);
        return BookIssue is null ? TypedResults.NotFound() : TypedResults.Ok(BookIssue);
    }

    public static IResult GetBookByReturnDate(BookIssueService bookIssueService, DateOnly ReturnDate)
    {
        IEnumerable<BookIssueDto> BookIssue = bookIssueService.GetBookByReturnDate(ReturnDate);
        return BookIssue is null ? TypedResults.NotFound() : TypedResults.Ok(BookIssue);
    }

    private static IResult CreateBookIssueRequest(BookIssueService bookIssueservice, CreateBookIssueRequest request)
    {
        BookIssueDto? result = bookIssueservice.CreateBookIssueRequest(request);
        return result is null
            ? TypedResults.Problem("There was some problem. See log for more details.")
            : TypedResults.Ok(result);
    }

    private static IResult PatchBookIssueRequest(BookIssueService bookIssueservice, PatchBookIssueRequest request,
        int IssueId)
    {
        BookIssueDto? result = bookIssueservice.PatchBookIssueRequest(request, IssueId);

        return result is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(result);
    }

    private static IResult PatchRenewedBookIssueRequest(
        BookIssueService bookIssueService,
        int IssueId,
        PatchRenewedBookIssueRequest request)
    {
        BookIssueDto? result = bookIssueService.PatchRenewedBookIssueRequest(request, IssueId);
        return result is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(result);
    }

    private static IResult DeleteBookIssue(BookIssueService service, int issueId)
    {
        try
        {
            BookIssueDto result = service.DeleteBookIssue(issueId);
            return TypedResults.Ok(result);
        }
        catch (ConflictException)
        {
            return TypedResults.NotFound();
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }

    private static IResult UpdateBookIssue(
        BookIssueService bookIssueService,
        int issueId,
        CreateBookIssueRequest request)
    {
        if (request.MemberId <= 0)
        {
            return TypedResults.BadRequest("Valid MemberId is required");
        }

        if (request.BookId <= 0)
        {
            return TypedResults.BadRequest("Valid BookId is required");
        }

        BookIssueDto? result = bookIssueService.UpdateBookIssue(issueId, request);

        return result is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(result);
    }
}
