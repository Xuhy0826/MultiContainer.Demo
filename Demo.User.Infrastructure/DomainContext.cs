using Demo.User.Infrastructure.EntityConfigurations;
using DotNetCore.CAP;
using Infrastructure.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Demo.User.Infrastructure
{
    public class DomainContext : EFContext
    {
        public DomainContext(DbContextOptions options, IMediator mediator, ICapPublisher capBus)
            : base(options, mediator, capBus)
        {
        }

        public DbSet<Domain.Aggregate.User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region 注册领域模型与数据库的映射关系
            modelBuilder.ApplyConfiguration(new UserEntityTypeConfiguration());
            #endregion

            base.OnModelCreating(modelBuilder);
        }
    }
}
