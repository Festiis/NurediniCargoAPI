using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NurediniCargoAPI.Entities;

public class WarehouseConfiguration : BaseConfiguration<Warehouse>
{
    public override void Configure(EntityTypeBuilder<Warehouse> builder)
    {
        base.Configure(builder);

        builder.Property(w => w.Name).IsRequired().HasMaxLength(100);
        builder.Property(w => w.Address).HasMaxLength(100);
        builder.Property(w => w.City).HasMaxLength(50);
        builder.Property(w => w.PostalCode).HasMaxLength(20);
        builder.Property(w => w.Country).HasMaxLength(50);

    }
}
