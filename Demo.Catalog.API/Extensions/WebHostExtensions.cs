using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Polly;

namespace Demo.Catalog.API.Extensions
{
    public static class WebHostExtensions
    {
        public static bool IsInKubernetes(this IWebHost host)
        {
            var cfg = host.Services.GetService<IConfiguration>();
            var orchestratorType = cfg.GetValue<string>("OrchestratorType");
            return orchestratorType?.ToUpper() == "K8S";
        }

        public static IWebHost MigrateDbContext<TContext>(this IWebHost host, Action<TContext, IServiceProvider> seeder) where TContext : DbContext
        {
            var underK8s = host.IsInKubernetes();
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<TContext>>();
                var context = services.GetService<TContext>();
                try
                {
                    logger.LogInformation("Migrating database associated with context {DbContextName}", typeof(TContext).Name);
                    if (underK8s)
                    {
                        InvokeSeeder(seeder, context, services);
                    }
                    else
                    {
                        var retry = Policy.Handle<MySqlException>()
                             .WaitAndRetry(new TimeSpan[]
                             {
                                 TimeSpan.FromSeconds(3),
                                 TimeSpan.FromSeconds(5),
                                 TimeSpan.FromSeconds(8),
                             });
                        retry.Execute(() => InvokeSeeder(seeder, context, services));
                    }
                    logger.LogInformation("Migrated database associated with context {DbContextName}", typeof(TContext).Name);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while migrating the database used on context {DbContextName}", typeof(TContext).Name);
                    if (underK8s)
                    {
                        throw;      // Rethrow under k8s because we rely on k8s to re-run the pod
                    }
                }
            }
            return host;
        }

        private static void InvokeSeeder<TContext>(Action<TContext, IServiceProvider> seeder, TContext context, IServiceProvider services)
            where TContext : DbContext
        {
            context.Database.Migrate();
            seeder(context, services);
        }
    }
}
