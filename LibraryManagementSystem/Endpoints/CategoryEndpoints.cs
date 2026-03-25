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
        endpoints.MapGet("Category/{categoryId:int}", GetCategoryByID);
        endpoints.MapPost("Category", CreateCategoryRequest);
        endpoints.MapPut("Category/{categoryId:int}", UpdateCategory);
        endpoints.MapDelete("Category/{categoryId:int}", DeleteCategory);
        return endpoints;
    }

    private static Ok<IEnumerable<CategoryDto>> GetCategory(CategoryService categoryService)
    {
        IEnumerable<CategoryDto> Category = categoryService.GetCategoriesList();
        return TypedResults.Ok(Category);
    }

    private static IResult GetCategoryByID(CategoryService categoryService, int categoryId)
    {
        CategoryDto? Category = categoryService.GetCategoryByID(categoryId);
        return Category is null ? TypedResults.NotFound() : TypedResults.Ok(Category);
    }

    private static IResult CreateCategoryRequest(CategoryService categoryService, CreateCategoryRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.CategoryName))
        {
            return TypedResults.BadRequest("Category Name is required.");
        }

        CategoryDto? result = categoryService.CreateCategoryRequest(request);
        return result is null
            ? TypedResults.Problem("There was some problem. See log for more details.")
            : TypedResults.Ok(result);
    }

    private static IResult DeleteCategory(CategoryService categoryService, int categoryId)
    {
        try
        {
            CategoryDto category = categoryService.DeleteCategoryRequest(categoryId);
            return TypedResults.Ok(category);
        }
        catch (KeyNotFoundException e)
        {
            return TypedResults.NotFound();
        }
    }

    private static IResult UpdateCategory(
        CategoryService categoryService,
        int categoryId,
        CreateCategoryRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.CategoryName))
        {
            return TypedResults.BadRequest("Category Name is required.");
        }

        CategoryDto? result = categoryService.UpdateCategory(categoryId, request);

        return result is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(result);
    }
}
