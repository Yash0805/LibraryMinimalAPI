using Azure.Core;
using LibraryManagementSystem.Core.Dtos;
using LibraryManagementSystem.Core.Request;
using LibraryManagementSystem.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LibraryManagementSystem.Web.Endpoints;

public static class CategoryEndpoints
{
    public static IEndpointRouteBuilder MapCategoryEndpoints(this IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);
        endpoints.MapGet("Category", GetCategory);
        endpoints.MapGet("Category/{CategoryId}", GetCategoryByID);
        endpoints.MapPost("Category", CreateCategoryRequest);
        return endpoints;
    }

    private static Ok<IEnumerable<CategoryDto>> GetCategory(CategoryService categoryService)
    {
        var Category = categoryService.GetCategoriesList();
        return TypedResults.Ok(Category);
    }

    private static IResult GetCategoryByID(CategoryService categoryService, int CategoryId)
    {
        var Category = categoryService.GetCategoryByID(CategoryId);
        return Category is null ? TypedResults.NotFound() : TypedResults.Ok(Category);
    }

    private static IResult CreateCategoryRequest(CategoryService categoryService, CreateCategoryRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.CategoryName))
            return TypedResults.BadRequest("Category Name is required.");
        var result = categoryService.CreateCategoryRequest(request); 
        return result is null
            ? TypedResults.Problem("There was some problem. See log for more details.")
            : TypedResults.Ok(result);
    }

}