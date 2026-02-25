using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Persistence;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Books> Books { get; init; }
    public DbSet<BookIssue> BookIssue { get; init; }
    public DbSet<Category> Category { get; init; }
    public DbSet<Members> Members { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var t = typeof(AppDbContext);
        modelBuilder.ApplyConfigurationsFromAssembly(t.Assembly);

        base.OnModelCreating(modelBuilder);
    }
}