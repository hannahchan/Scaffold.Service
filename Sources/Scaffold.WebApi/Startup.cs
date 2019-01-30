namespace Scaffold.WebApi
{
    using System;
    using System.IO;
    using System.Reflection;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Scaffold.Data;
    using Scaffold.WebApi.Extensions;
    using Scaffold.WebApi.Filters;
    using Swashbuckle.AspNetCore.Swagger;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHealthChecks()
                .AddDbContextCheck<BucketContext>();

            services
                .AddDbContext(this.Configuration)
                .AddOptions(this.Configuration)
                .AddRepositories()
                .AddServices()
                .AddUtilities();

            services.AddMvcCore(options => options.Filters.Add(new ExceptionFilter()))
                .AddApiExplorer()
                .AddCustomJsonFormatters()
                .AddCustomXmlFormatters()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSwaggerGen(options =>
            {
                Info info = new Info
                {
                    Version = "v1",
                    Title = "Scaffold.WebApi",
                    Description = "A simple CRUD application to demonstrate Scaffold.WebApi."
                };

                options.SwaggerDoc("v1", info);

                string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app
                .UseMiddleware()
                .UseMvc();
        }
    }
}
