using Loyalty.Infrastructure.External.Database.Context.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Loyalty.Infrastructure.External.Database.Context.Mappings;

public abstract class BaseMapping<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : BaseEntity
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder
            .Property(e => e.Version)
            .HasColumnType("xid")
            .HasColumnName("xmin")
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();

        builder
            .Property(e => e.CreationDateTime)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("creation_datetime");

        builder
            .Property(e => e.MutationDateTime)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("mutation_datetime");
    }
}
