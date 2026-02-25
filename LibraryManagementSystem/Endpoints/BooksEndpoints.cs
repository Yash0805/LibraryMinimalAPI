using LibraryManagementSystem.Core.Dtos;
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
        return endpoint;
    }

    private static Ok<IEnumerable<BooksDto>> GetBooks(BooksService booksService)
    {
        var Book = booksService.GetBooksList();
        return TypedResults.Ok(Book);
    }

    private static IResult GetBookById(BooksService booksService, int BookId)
    {
        var Book = booksService.GetBooksById(BookId);
        return Book is null ? TypedResults.NotFound() : TypedResults.Ok(Book);
    }
}