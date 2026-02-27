using LibraryManagementSystem.Core.Dtos;
using LibraryManagementSystem.Core.Request;
using LibraryManagementSystem.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LibraryManagementSystem.Services;

public sealed class CategoryService
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<CategoryService> _logger;

    public CategoryService(AppDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public IEnumerable<CategoryDto> GetCategoriesList()
    {
        IReadOnlyList<CategoryDto> Category = _dbContext.Category
            .Select
            (c => new CategoryDto
            (
                c.CategoryId,
                c.CategoryName
            ))
            .ToList();
        return Category;
    }

    public CategoryDto? GetCategoryByID(int CategoryId)
    {
        var Category = _dbContext.Category.FirstOrDefault(c => c.CategoryId == CategoryId);
        if (Category is null) return null;
        return new CategoryDto(
            Category.CategoryId,
            Category.CategoryName);
    }

    public CategoryDto? CreateCategoryRequest(CreateCategoryRequest request)
    {
        try
        {
            var Category = new Category
            {
                CategoryName = request.CategoryName
            };
            _dbContext.Category.Add(Category);
            _dbContext.SaveChanges();

            var CategoryDto = new CategoryDto(
                Category.CategoryId,
                Category.CategoryName);
            return CategoryDto;
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error while creating category for category name {CategoryName}",
                request.CategoryName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Unexpected error while creating category for category name {CategoryName} ",
               request.CategoryName);
        }
        return null;
    }
}