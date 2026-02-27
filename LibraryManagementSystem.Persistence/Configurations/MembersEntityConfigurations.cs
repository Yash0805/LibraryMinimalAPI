using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagementSystem.Persistence.Configurations;

public sealed class MembersEntityConfigurations : IEntityTypeConfiguration<Members>
{
    public void Configure(EntityTypeBuilder<Members> builder)
    {
        builder.ToTable("Members");

        builder.HasKey(m => m.MemberId);

        builder.Property(m => m.MemberName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(m => m.MemberType)
               .IsRequired()
               .HasMaxLength(20);
        
    }
}