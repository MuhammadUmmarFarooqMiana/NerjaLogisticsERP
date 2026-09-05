using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Infrastructure.Data.Configurations;

public class CompanyDocumentConfiguration : IEntityTypeConfiguration<CompanyDocument>
{
    public void Configure(EntityTypeBuilder<CompanyDocument> builder)
    {
        builder.Property(d => d.Title).HasMaxLength(200).IsRequired();
        builder.Property(d => d.Category).HasConversion<string>().HasMaxLength(30);
        builder.Property(d => d.StorageKey).HasMaxLength(500).IsRequired();

        builder.HasOne(d => d.Folder)
            .WithMany()
            .HasForeignKey(d => d.FolderId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
