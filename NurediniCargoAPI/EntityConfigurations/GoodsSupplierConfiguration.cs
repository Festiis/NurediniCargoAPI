using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NurediniCargoAPI.Entities;

public class GoodsSupplierConfiguration : IEntityTypeConfiguration<GoodsSupplier>
{
    public void Configure(EntityTypeBuilder<GoodsSupplier> builder)
    {
        builder.HasKey(gs => new { gs.GoodsId, gs.SupplierId });

        builder.HasOne(gs => gs.Goods)
            .WithMany(g => g.GoodsSupplier)
            .HasForeignKey(gs => gs.GoodsId);

        builder.HasOne(gs => gs.Supplier)
            .WithMany(s => s.SupplierGoods)
            .HasForeignKey(gs => gs.SupplierId);
    }
}
