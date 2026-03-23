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
        Category? Category = _dbContext.Category.FirstOrDefault(c => c.CategoryId == CategoryId);
        if (Category is null)
        {
            return null;
        }

        return new CategoryDto(
            Category.CategoryId,
            Category.CategoryName);
    }

    public CategoryDto? CreateCategoryRequest(CreateCategoryRequest request)
    {
        try
        {
            Category Category = new() { CategoryName = request.CategoryName };
            _dbContext.Category.Add(Category);
            _dbContext.SaveChanges();

            CategoryDto CategoryDto = new(
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

    public CategoryDto DeleteCategoryRequest(int categoryId)
    {
        try
        {
            Category? category = _dbContext.Category
                .FirstOrDefault(c => c.CategoryId == categoryId);

            if (category is null)
            {
                throw new ConflictException($"Category with ID {categoryId} not found.");
            }

            _dbContext.Category.Remove(category);
            _dbContext.SaveChanges();

            return new CategoryDto(
                category.CategoryId,
                category.CategoryName
            );
        }
        catch (ConflictException ex)
        {
            _logger.LogError(ex, "Category not found with ID {CategoryId}", categoryId);
            throw;
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error while deleting category with ID {CategoryId}", categoryId);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while deleting category with ID {CategoryId}", categoryId);
            throw;
        }
    }

    public CategoryDto? UpdateCategory(int categoryId, CreateCategoryRequest request)
    {
        try
        {
            Category? category = _dbContext.Category
                .FirstOrDefault(c => c.CategoryId == categoryId);

            if (category is null)
            {
                _logger.LogWarning("Category not found with ID {CategoryId}", categoryId);
                return null;
            }

            category.CategoryName = request.CategoryName;

            _dbContext.SaveChanges();

            return new CategoryDto(
                category.CategoryId,
                category.CategoryName
            );
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex,
                "Database error while updating category with ID {CategoryId}", categoryId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Unexpected error while updating category with ID {CategoryId}", categoryId);
        }

        return null;
    }
}
