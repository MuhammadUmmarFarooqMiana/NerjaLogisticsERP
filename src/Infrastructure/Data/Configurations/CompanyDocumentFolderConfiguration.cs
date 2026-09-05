using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Infrastructure.Data.Configurations;

public class CompanyDocumentFolderConfiguration : IEntityTypeConfiguration<CompanyDocumentFolder>
{
    public void Configure(EntityTypeBuilder<CompanyDocumentFolder> builder)
    {
        builder.Property(f => f.Name).HasMaxLength(150).IsRequired();
        builder.HasIndex(f => new { f.ParentFolderId, f.Name });

        builder.HasOne(f => f.ParentFolder)
            .WithMany()
            .HasForeignKey(f => f.ParentFolderId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
