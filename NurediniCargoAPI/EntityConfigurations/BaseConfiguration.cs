using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NurediniCargoAPI.Entities;

public class BaseConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : EntityBase
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {

        builder.Property(e => e.CreatedBy).IsRequired();

        // Set CreatedDate to the current timestamp when an entity is added
        builder.Property(e => e.CreatedDate)
            .IsRequired()
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("GETDATE()");

        builder.Property(e => e.LastModifiedBy).IsRequired();
        builder.Property(e => e.LastModifiedDate)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("GETDATE()"); ;

        // Add other common configurations if needed
    }
}
