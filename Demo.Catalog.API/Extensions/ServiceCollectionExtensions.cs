using System.Reflection;
using Demo.Catalog.API.Infrastructure;
using Demo.Catalog.API.Infrastructure.Filter;
using Demo.Catalog.API.IntegrationEvents;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.Catalog.API.ServiceExtension
{
    public static class ServiceCollectionExtensions
    {
        #region MVC
        public static IServiceCollection AddCustomMVC(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers(options =>
            {
                options.Filters.Add(typeof(HttpGlobalExceptionFilter));
            }).AddNewtonsoftJson();

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                        .SetIsOriginAllowed((host) => true)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });

            return services;
        }
        #endregion

        #region 数据库
        public static IServiceCollection AddCustomDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddEntityFrameworkMySql()
                .AddDbContext<CatalogContext>(options =>
                {
                    options.UseMySql(configuration["MySql"],
                        sqlOptions =>
                        {
                            sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                        });
                });
            return services;
        }
        #endregion

        #region 配置

        public static IServiceCollection AddCustomOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<CatalogSettings>(configuration);
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var problemDetails = new ValidationProblemDetails(context.ModelState)
                    {
                        Instance = context.HttpContext.Request.Path,
                        Status = StatusCodes.Status400BadRequest,
                        Detail = "Please refer to the errors property for additional details."
                    };

                    return new BadRequestObjectResult(problemDetails)
                    {
                        ContentTypes = { "application/problem+json", "application/problem+xml" }
                    };
                };
            });

            return services;
        }
        #endregion

        #region 集成事件
        public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<ICatalogIntegrationEventService, CatalogIntegrationEventService>();
            services.AddCap(options =>
            {
                options.FailedRetryCount = 1;
                options.UseEntityFramework<CatalogContext>();
                _ = options.UseRabbitMQ(rabbiteOptions =>
                {
                    configuration.GetSection("RabbitMQ").Bind(options);
                });
                //options.UseDashboard();
            });
            return services;
        }
        #endregion
    }
}
