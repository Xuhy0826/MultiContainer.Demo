using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MySql.Data.EntityFrameworkCore.Extensions;

namespace Demo.User.Infrastructure.EntityConfigurations
{
    public class UserEntityTypeConfiguration : IEntityTypeConfiguration<Domain.Aggregate.User>
    {
        public void Configure(EntityTypeBuilder<Domain.Aggregate.User> builder)
        {
            builder.HasKey(p => p.Id);
            builder.ToTable("Demo_User");
            builder.Property(p => p.Id).HasColumnName("ID");
            builder.Property(p => p.CreateTime).ForMySQLHasDefaultValue(DateTime.Now);
        }
    }
}
