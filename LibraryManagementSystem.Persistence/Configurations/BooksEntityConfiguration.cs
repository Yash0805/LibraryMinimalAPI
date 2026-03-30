using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagementSystem.Persistence.Configurations;

public sealed class BooksEntityConfiguration : IEntityTypeConfiguration<Books>
{
    public void Configure(EntityTypeBuilder<Books> builder)
    {
        builder.ToTable("Books");

        builder.HasKey(b => b.BookId);

        builder.Property(b => b.BookName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(b => b.Publisher)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(b => b.Author)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(b => b.Price)
            .HasPrecision(10, 2)
            .IsRequired();

        builder.HasOne(c => c.Category)
            .WithMany(b => b.Books)
            .HasForeignKey(c => c.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(i => i.BookIssue).WithOne(m => m.Book).HasForeignKey(m => m.BookId).OnDelete(DeleteBehavior.Restrict);
    }
}
