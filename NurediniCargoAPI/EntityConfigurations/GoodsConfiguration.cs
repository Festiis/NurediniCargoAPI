// GoodsConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NurediniCargoAPI.Entities;
using System.Reflection.Emit;

namespace NurediniCargoAPI.EntityConfigurations
{
    public class GoodsConfiguration : BaseConfiguration<Goods>
    {
        public override void Configure(EntityTypeBuilder<Goods> builder)
        {
            base.Configure(builder);

            // Add specific configurations for Goods entity
            builder.Property(g => g.Name)
                .IsRequired()
                .HasMaxLength(255);
            builder.Property(g => g.Price)
                .HasColumnType("decimal(10, 2)")
                .IsRequired()
                .HasDefaultValue(0);
            builder.Property(g => g.Description).HasMaxLength(500);
            builder.Property(g => g.MinimumOrderQuantity).HasDefaultValue(1);
            builder.Property(g => g.ShippingClass).HasMaxLength(50);
            builder.Property(g => g.HandlingInstructions).HasMaxLength(200);
            builder.Property(g => g.Brand).HasMaxLength(50);
            builder.Property(g => g.Category).HasMaxLength(50);
            builder.Property(g => g.UnitOfMeasure).HasMaxLength(20);

        }
    }
}