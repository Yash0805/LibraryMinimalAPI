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
        endpoint.MapGet("Books/{bookId:int}", GetBookById);   
        endpoint.MapPost("Books", CreateBooksRequest);
        endpoint.MapDelete("Books/{bookId:int}", DeleteBook);
        endpoint.MapPut("Books/{bookId:int}", UpdateBook);

        return endpoint;
    }

    private static Ok<IEnumerable<BooksDto>> GetBooks(
        BooksService booksService,
        string? bookName)
    {
        var books = booksService.GetBooksList(bookName);
        return TypedResults.Ok(books);
    }

    private static IResult GetBookById(
        BooksService booksService,
        int bookId)
    {
        var book = booksService.GetBooksById(bookId);
        return book is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(book);
    }

    private static IResult CreateBooksRequest(
        BooksService booksService,
        CreateBooksRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.BookName))
            return TypedResults.BadRequest("Book Name is required.");

        if (string.IsNullOrWhiteSpace(request.Publisher))
            return TypedResults.BadRequest("Publisher is required.");

        if (string.IsNullOrWhiteSpace(request.Author))
            return TypedResults.BadRequest("Author is required.");

        if (request.CategoryId <= 0)  
            return TypedResults.BadRequest("Category is required.");

        var result = booksService.CreateBooksRequest(request);

        return result is null
            ? TypedResults.Problem("Error while creating book.")
            : TypedResults.Ok(result);
    }

    private static IResult DeleteBook(
        BooksService booksService,
        int bookId)
    {
        try
        {
            var book = booksService.DeleteBooksRequest(bookId);
            return TypedResults.Ok(book);
        }
        catch (Exception)
        {
            return TypedResults.NotFound();
        }
    }

    private static IResult UpdateBook(
        BooksService booksService,
        int bookId,
        CreateBooksRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.BookName))
            return TypedResults.BadRequest("Book Name is required");

        if (request.CategoryId <= 0)   
            return TypedResults.BadRequest("Category is required");

        var result = booksService.UpdateBook(bookId, request);

        return result is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(result);
    }
}
