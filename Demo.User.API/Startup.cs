using System;
using System.IO;
using System.Linq;
using AutoMapper;
using Demo.User.API.Extensions;
using Demo.User.API.SwaggerExt;
using Domain.Core.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;

namespace Demo.User.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(setup =>
            {
                setup.ReturnHttpNotAcceptable = true; //是否返回406（unacceptable response type)
            })
            .AddNewtonsoftJson(setup =>
             {
                 setup.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
             })
            .AddXmlDataContractSerializerFormatters()
            .ConfigureApiBehaviorOptions(setup =>
            {
                //发生客户端错误
                setup.InvalidModelStateResponseFactory = context =>
                {
                    var problemDetails = new ValidationProblemDetails(context.ModelState)
                    {
                        Type = "http://www.google.com",
                        Title = "有错误！！！",
                        Status = StatusCodes.Status422UnprocessableEntity,
                        Detail = "请看详细信息",
                        Instance = context.HttpContext.Request.Path
                    };

                    problemDetails.Extensions.Add("traceId", context.HttpContext.TraceIdentifier);

                    return new UnprocessableEntityObjectResult(problemDetails)
                    {
                        ContentTypes = { "application/problem+json" }
                    };
                };
            });

            services.Configure<MvcOptions>(config =>
            {
                var newtonSoftJsonOutputFormatter =
                    config.OutputFormatters.OfType<NewtonsoftJsonOutputFormatter>()?.FirstOrDefault();
                newtonSoftJsonOutputFormatter?.SupportedMediaTypes.Add("application/vnd.kwitd.hateoas+json");
            });

            //注入中间者
            services.AddMediatRServices();
            //注入数据可支持
            services.AddMySqlDomainContext(Configuration.GetValue<string>("Oracle"), Configuration.GetValue<string>("OracleVersion"));
            //注入仓储
            services.AddRepositories();
            services.AddEventBus(Configuration);
            //注入AutoMapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            //注入自定义的一些服务
            services.AddCommonServices();
            services.AddMvc();
            //添加API文档
            services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("ReturnVisit.WebApi",
                    new OpenApiInfo
                    {
                        Title = "ReturnVisit.WebApi",
                        Version = "v1.0",
                        Contact = new OpenApiContact { Name = "xuhy", Email = "hyxu0826@gmail.com" },
                        Description = "维护“复诊”数据的WebApi接口"
                    });
                //添加中文注释
                var basePath = Path.GetDirectoryName(typeof(Program).Assembly.Location);
                var commentsFileNameMain = typeof(Program).Assembly.GetName().Name + ".XML";
                var commentsFileNameModel = typeof(Domain.Aggregate.User).Assembly.GetName().Name + ".XML";
                var commentsFileNameModelBase = typeof(QueryModel).Assembly.GetName().Name + ".XML";

                commentsFileNameMain = Path.Combine(basePath, commentsFileNameMain);
                if (File.Exists(commentsFileNameMain))
                    option.IncludeXmlComments(commentsFileNameMain);

                commentsFileNameModel = Path.Combine(basePath, commentsFileNameModel);
                if (File.Exists(commentsFileNameModel))
                    option.IncludeXmlComments(commentsFileNameModel);

                commentsFileNameModelBase = Path.Combine(basePath, commentsFileNameModelBase);
                if (File.Exists(commentsFileNameModelBase))
                    option.IncludeXmlComments(commentsFileNameModelBase);

                option.DocInclusionPredicate((docName, description) => true);
                //注册过滤器
                option.OperationFilter<SwaggerOperationFilter>();
            });
            //如果项目的Json库使用的是Newtonsoft，需要在services.AddSwaggerGen()后显示注入下面的服务
            services.AddSwaggerGenNewtonsoftSupport();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(appBuilder =>
                {
                    appBuilder.Run(async context =>
                    {
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("Unexpected Error!");
                    });
                });
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // 启用Swagger中间件
            app.UseSwagger();
            // 配置SwaggerUI
            var virtualPath = Configuration["virtualPath"];
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(virtualPath + "/swagger/ReturnVisit.WebApi/swagger.json", "ReturnVisit.WebApi");
                c.RoutePrefix = string.Empty;

            });
        }
    }
}
