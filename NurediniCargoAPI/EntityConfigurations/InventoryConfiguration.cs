// InventoryConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NurediniCargoAPI.Entities;

public class InventoryConfiguration : BaseConfiguration<Inventory>
{
    public override void Configure(EntityTypeBuilder<Inventory> builder)
    {
        base.Configure(builder);

        // Add specific configurations for Inventory entity
        builder.Property(i => i.WarehouseId).IsRequired();
        builder
            .HasOne(i => i.Warehouse)
            .WithMany()
            .HasForeignKey(i => i.WarehouseId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(i => i.GoodsId).IsRequired();
        builder
            .HasOne(i => i.Goods)
            .WithMany()
            .HasForeignKey(i => i.GoodsId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        // Set Quantity to be greater than or equal to zero
        builder.Property(i => i.Quantity).IsRequired();

        builder.ToTable("Inventory", t => t.HasCheckConstraint("CK_Inventory_Quantity", "Quantity >= 0"));

    }
}
