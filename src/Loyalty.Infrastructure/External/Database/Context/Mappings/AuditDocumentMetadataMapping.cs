using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Loyalty.Infrastructure.External.Database.Context.Entities;

namespace Loyalty.Infrastructure.External.Database.Context.Mappings;
public class AuditDocumentMetadataMapping : IEntityTypeConfiguration<AuditDocumentMetadata>
{
    public void Configure(EntityTypeBuilder<AuditDocumentMetadata> builder)
    {
        builder.ToTable("audit_document_metadata");

        builder
            .HasKey(e => e.DocumentId);

        builder.Property(e => e.DocumentId)
            .HasColumnName("document_id");

        builder.Property(e => e.Type)
            .HasColumnName("type");

        builder.Property(e => e.Name)
            .HasColumnName("name");

        builder.Property(e => e.Timestamp)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("timestamp");

        builder.Property(e => e.Username)
            .HasColumnName("username");

        builder.Property(e => e.CorrelationId)
            .HasColumnName("correlation_id");
        
        builder.Property(e => e.OrderNumber)
            .HasColumnName("order_number");

        builder.Property(e => e.AtlasLabel)
            .HasColumnName("atlas_label");
    }
}
