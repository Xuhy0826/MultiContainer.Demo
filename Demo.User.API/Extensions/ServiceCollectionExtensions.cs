using System;
using Demo.User.API.Application.IntegrationEvents;
using Demo.User.API.Services;
using Demo.User.Infrastructure;
using Mark.Common.Helper;
using Mark.Common.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Demo.User.API.Extensions
{
    /// <summary>
    /// 服务注册
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        #region 数据库相关
        public static IServiceCollection AddDomainContext(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsAction)
        {
            return services.AddDbContext<DomainContext>(optionsAction);
        }

        public static IServiceCollection AddOracleDomainContext(this IServiceCollection services, string connectionString, string version)
        {
            return services.AddDomainContext(builder =>
            {
                var loggerFactory = new LoggerFactory();
                loggerFactory.AddProvider(new EFLoggerProvider());
                builder.UseLoggerFactory(loggerFactory);
                builder.UseMySQL(connectionString);
            });
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            return services;
        }

        #endregion

        public static IServiceCollection AddMediatRServices(this IServiceCollection services)
        {
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(DomainContextTransactionBehavior<,>));
            return services.AddMediatR(typeof(Program).Assembly);
        }

        public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<ISubscriberService, SubscriberService>();
            services.AddCap(options =>
            {
                options.FailedRetryCount = 1;
                options.UseMySql(configuration["MySql"]);
                _ = options.UseRabbitMQ(rabbiteOptions =>
                {
                    configuration.GetSection("RabbitMQ").Bind(options);
                });
                //options.UseDashboard();
            });
            return services;
        }

        public static IServiceCollection AddCommonServices(this IServiceCollection services)
        {
            services.AddTransient<IPropertyMappingService, PropertyMappingService>();
            services.AddTransient<IPropertyCheckerService, PropertyCheckerService>();
            return services;
        }
    }
}
