using DotNetCore.CAP;
using Infrastructure.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Demo.$projectname$.Infrastructure
{
    public class DomainContext : EFContext
    {
        public DomainContext(DbContextOptions options, IMediator mediator, ICapPublisher capBus)
            : base(options, mediator, capBus)
        {
        }

        public DbSet<$projectname$> ReturnVisits { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region 注册领域模型与数据库的映射关系
            modelBuilder.ApplyConfiguration(new $projectname$EntityTypeConfiguration());
            #endregion

            base.OnModelCreating(modelBuilder);
        }
    }
}
