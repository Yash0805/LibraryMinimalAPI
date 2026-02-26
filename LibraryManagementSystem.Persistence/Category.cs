using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Persistence;

public sealed class Category
{
    [Key] public int CategoryId { get; set; }

    public required string CategoryName { get; set; }
    public IList<Books> Books { get; init; } = [];
}