using LibraryManagementSystem.Core.Dtos;
using LibraryManagementSystem.Core.Request;
using LibraryManagementSystem.Persistence;
using LibraryManagementSystem.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Runtime.CompilerServices;

namespace LibraryManagementSystem.Web.Endpoints;

public static class BooksEndpoints
{
    public static IEndpointRouteBuilder MapBooksEndpoints(this IEndpointRouteBuilder endpoint)
    {
        ArgumentNullException.ThrowIfNull(endpoint);
        endpoint.MapGet("Books", GetBooks);
        endpoint.MapGet("Books/{BookId}", GetBookById);
        endpoint.MapPost("Books", CreateBooksRequest);
        return endpoint;
    }

    private static Ok<IEnumerable<BooksDto>> GetBooks(BooksService booksService, string? BookName)
    {
        var Book = booksService.GetBooksList(BookName);
        return TypedResults.Ok(Book);
    }

    private static IResult GetBookById(BooksService booksService, int BookId)
    {
        var Book = booksService.GetBooksById(BookId);
        return Book is null ? TypedResults.NotFound() : TypedResults.Ok(Book);
    }
    private static IResult CreateBooksRequest(BooksService booksService, CreateBooksRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.BookName))
            return TypedResults.BadRequest("Book Name is required.");
        if (string.IsNullOrWhiteSpace(request.Publisher))
            return TypedResults.BadRequest(" Publisher is required.");
        if (string.IsNullOrWhiteSpace(request.Author))
            return TypedResults.BadRequest("Author is required.");
        var result = booksService.CreateBooksRequest(request);
        return result is null
            ? TypedResults.Problem("There was some problem. See log for more details.")
            : TypedResults.Ok(result);
    }

}