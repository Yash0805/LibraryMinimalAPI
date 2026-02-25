using LibraryManagementSystem.Core.Dtos;
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
}