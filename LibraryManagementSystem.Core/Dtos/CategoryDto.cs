namespace LibraryManagementSystem.Core.Dtos;

public sealed class CategoryDto(int CategoryId, string CategoryName)
{
    public int CategoryId { get; } = CategoryId;
    public string CategoryName { get; } = CategoryName;
}