using Demo.Catalog.API.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demo.Catalog.API.Infrastructure.EntityConfigurations
{ 
    class CatalogBrandEntityTypeConfiguration : IEntityTypeConfiguration<CatalogBrand>
    {
        public void Configure(EntityTypeBuilder<CatalogBrand> builder)
        {
            builder.ToTable("CatalogBrand");
            builder.HasKey(ci => ci.Id);
            //builder.Property(ci => ci.Id).IsRequired();
            builder.Property(cb => cb.Brand).IsRequired().HasMaxLength(100);
        }
    }
}
