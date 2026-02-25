using LibraryManagementSystem.Core.Dtos;
using LibraryManagementSystem.Persistence;

namespace LibraryManagementSystem.Services;

public sealed class CategoryService
{
    private readonly AppDbContext _dbContext;

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
}