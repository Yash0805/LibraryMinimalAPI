using LibraryManagementSystem.Core.Dtos;
using LibraryManagementSystem.Core.Request;
using LibraryManagementSystem.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LibraryManagementSystem.Web.Endpoints;

public static class BooksEndpoints
{
    public static IEndpointRouteBuilder MapBooksEndpoints(this IEndpointRouteBuilder endpoint)
    {
        ArgumentNullException.ThrowIfNull(endpoint);
        endpoint.MapGet("Books", GetBooks);
        endpoint.MapGet("Books/{BookId}", GetBookById);
        endpoint.MapPost("Books", CreateBooksRequest);
        endpoint.MapDelete("Books/{BookId}", DeleteBook);
        endpoint.MapPut("Books/{bookId:int}", UpdateBook);
        return endpoint;
    }

    private static Ok<IEnumerable<BooksDto>> GetBooks(BooksService booksService, string? BookName)
    {
        IEnumerable<BooksDto> Book = booksService.GetBooksList(BookName);
        return TypedResults.Ok(Book);
    }

    private static IResult GetBookById(BooksService booksService, int BookId)
    {
        BooksDto? Book = booksService.GetBooksById(BookId);
        return Book is null ? TypedResults.NotFound() : TypedResults.Ok(Book);
    }

    private static IResult CreateBooksRequest(BooksService booksService, CreateBooksRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.BookName))
        {
            return TypedResults.BadRequest("Book Name is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Publisher))
        {
            return TypedResults.BadRequest(" Publisher is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Author))
        {
            return TypedResults.BadRequest("Author is required.");
        }

        BooksDto? result = booksService.CreateBooksRequest(request);
        return result is null
            ? TypedResults.Problem("There was some problem. See log for more details.")
            : TypedResults.Ok(result);
    }

    private static IResult DeleteBook(BooksService booksService, int BookId)
    {
        try
        {
            BooksDto book = booksService.DeleteBooksRequest(BookId);
            return TypedResults.Ok(book);
        }
        catch (ConflictException)
        {
            return TypedResults.NotFound();
        }
        catch (Exception)
        {
            return TypedResults.Problem("Error while deleting book.");
        }
    }

    private static IResult UpdateBook(
        BooksService booksService,
        int bookId,
        CreateBooksRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.BookName))
        {
            return TypedResults.BadRequest("Book Name is required");
        }

        BooksDto? result = booksService.UpdateBook(bookId, request);

        return result is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(result);
    }
}
