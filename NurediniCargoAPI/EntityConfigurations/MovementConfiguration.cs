using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using NurediniCargoAPI.Entities;

namespace NurediniCargoAPI.EntityConfigurations
{
    public class MovementConfiguration : IEntityTypeConfiguration<Movement>
    {
        public void Configure(EntityTypeBuilder<Movement> builder)
        {
            builder.ToTable("Movements");

            builder.HasKey(m => m.Id);
            builder.Property(m => m.Id).HasColumnName("Id").ValueGeneratedOnAdd();

            builder.Property(m => m.GoodsId).IsRequired();
            builder.Property(m => m.SourceWarehouseId).IsRequired();
            builder.Property(m => m.DestinationWarehouseId).IsRequired();
            builder.Property(m => m.Quantity).IsRequired();
            builder.Property(m => m.MovementDate).IsRequired();
            builder.Property(m => m.MovementBy).IsRequired();

            builder.Property(m => m.SupplierId);

            // Relationships
            builder.HasOne(m => m.Goods)
                .WithMany()
                .HasForeignKey(m => m.GoodsId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(m => m.SourceWarehouse)
                .WithMany()
                .HasForeignKey(m => m.SourceWarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(m => m.DestinationWarehouse)
                .WithMany()
                .HasForeignKey(m => m.DestinationWarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(m => m.Supplier)
                .WithMany()
                .HasForeignKey(m => m.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
