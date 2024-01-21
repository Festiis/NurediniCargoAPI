using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NurediniCargoAPI.Entities;

public class SupplierConfiguration : BaseConfiguration<Supplier>
{
    public override void Configure(EntityTypeBuilder<Supplier> builder)
    {
        base.Configure(builder);

        // Add specific configurations for Supplier entity
        builder.Property(s => s.Name).IsRequired().HasMaxLength(100);
        builder.Property(s => s.ContactPerson).HasMaxLength(50);
        builder.Property(s => s.Email).HasMaxLength(100);
        builder.Property(s => s.Phone).HasMaxLength(20);
        builder.Property(s => s.Address).IsRequired().HasMaxLength(100);
        builder.Property(s => s.City).IsRequired().HasMaxLength(50);
        builder.Property(s => s.PostalCode).IsRequired().HasMaxLength(20);
        builder.Property(s => s.Country).HasMaxLength(50);

    }
}
