using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagementSystem.Persistence.Configurations;

public sealed class CategoryEntityConfigurations : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Category");
        builder.HasKey(c => c.CategoryId);

        builder.Property(c => c.CategoryName)
            .HasMaxLength(100)
            .IsRequired();
    }
}

