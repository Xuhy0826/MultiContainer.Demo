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
                setup.ReturnHttpNotAcceptable = true; //�Ƿ񷵻�406��unacceptable response type)
            })
            .AddNewtonsoftJson(setup =>
             {
                 setup.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
             })
            .AddXmlDataContractSerializerFormatters()
            .ConfigureApiBehaviorOptions(setup =>
            {
                //�����ͻ��˴���
                setup.InvalidModelStateResponseFactory = context =>
                {
                    var problemDetails = new ValidationProblemDetails(context.ModelState)
                    {
                        Type = "http://www.google.com",
                        Title = "�д��󣡣���",
                        Status = StatusCodes.Status422UnprocessableEntity,
                        Detail = "�뿴��ϸ��Ϣ",
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

            //ע���м���
            services.AddMediatRServices();
            //ע�����ݿ�֧��
            services.AddMySqlDomainContext(Configuration.GetValue<string>("Oracle"), Configuration.GetValue<string>("OracleVersion"));
            //ע��ִ�
            services.AddRepositories();
            services.AddEventBus(Configuration);
            //ע��AutoMapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            //ע���Զ����һЩ����
            services.AddCommonServices();
            services.AddMvc();
            //���API�ĵ�
            services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("ReturnVisit.WebApi",
                    new OpenApiInfo
                    {
                        Title = "ReturnVisit.WebApi",
                        Version = "v1.0",
                        Contact = new OpenApiContact { Name = "xuhy", Email = "hyxu0826@gmail.com" },
                        Description = "ά����������ݵ�WebApi�ӿ�"
                    });
                //�������ע��
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
                //ע�������
                option.OperationFilter<SwaggerOperationFilter>();
            });
            //�����Ŀ��Json��ʹ�õ���Newtonsoft����Ҫ��services.AddSwaggerGen()����ʾע������ķ���
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

            // ����Swagger�м��
            app.UseSwagger();
            // ����SwaggerUI
            var virtualPath = Configuration["virtualPath"];
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(virtualPath + "/swagger/ReturnVisit.WebApi/swagger.json", "ReturnVisit.WebApi");
                c.RoutePrefix = string.Empty;

            });
        }
    }
}
