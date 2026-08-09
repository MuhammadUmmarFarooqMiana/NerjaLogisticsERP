using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Infrastructure.Data.Configurations;

public class InventoryItemConfiguration : IEntityTypeConfiguration<InventoryItem>
{
    public void Configure(EntityTypeBuilder<InventoryItem> builder)
    {
        builder.Property(i => i.ItemName).HasMaxLength(150).IsRequired();
        builder.HasIndex(i => i.ItemName).IsUnique();
        builder.Property(i => i.Unit).HasMaxLength(20);
        builder.ToTable(t => t.HasCheckConstraint("CK_InventoryItems_CurrentStock_NonNegative", "\"CurrentStock\" >= 0"));
    }
}
