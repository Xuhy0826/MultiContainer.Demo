using Demo.Catalog.API.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demo.Catalog.API.Infrastructure.EntityConfigurations
{
    class CatalogTypeEntityTypeConfiguration
        : IEntityTypeConfiguration<CatalogType>
    {
        public void Configure(EntityTypeBuilder<CatalogType> builder)
        {
            builder.ToTable("CatalogType");

            builder.HasKey(ci => ci.Id);

            builder.Property(ci => ci.Id).IsRequired();

            builder.Property(cb => cb.TypeName)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(cb => cb.TypeDescription).HasMaxLength(2000);
        }
    }
}
