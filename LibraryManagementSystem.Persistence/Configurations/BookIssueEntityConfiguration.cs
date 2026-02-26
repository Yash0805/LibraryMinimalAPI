using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagementSystem.Persistence.Configurations;
public sealed class BookIssueEntityConfiguration : IEntityTypeConfiguration<BookIssue>
{
    public void Configure(EntityTypeBuilder<BookIssue> builder)
    {
        builder.ToTable("BookIssue");
        builder.HasKey(bi => bi.IssueId);

        builder.HasOne(m => m.Member)
        .WithMany(b => b.BookIssue)
        .HasForeignKey(m => m.MemberId)
        .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.Book)
        .WithMany(b => b.BookIssue)
        .HasForeignKey(b => b.BookId)
         .OnDelete(DeleteBehavior.Restrict);

        builder.Property(bi => bi.IssueDate)
            .IsRequired();

        builder.Property(bi => bi.RenewCount)
            .IsRequired()
            .HasDefaultValue(0);
        builder.ToTable(bi =>
        {
            bi.HasCheckConstraint(
            "CK_BookIssue_RenewCount",
            "RenewCount >= 0 AND RenewCount <= 1"
            );
        });

        builder.Property(bi => bi.Status)
            .IsRequired()
            .HasDefaultValue("Issued")
            .HasMaxLength(20);

        builder.ToTable(bi =>
        {
            bi.HasCheckConstraint(
                "CK_BookIssue_Status",
                "Status IN ('Issued', 'Returned','Renewed')"
            );
        });
    }
}

