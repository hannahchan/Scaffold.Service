namespace Scaffold.WebApi
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Scaffold.Data;
    using Scaffold.WebApi.Extensions;
    using Scaffold.WebApi.Filters;

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
                .AddHttpClients()
                .AddOptions(this.Configuration)
                .AddRepositories()
                .AddServices()
                .AddUtilities();

            services.AddMvcCore(options => options.Filters.Add<ExceptionFilter>())
                .AddApiExplorer()
                .AddCustomJsonFormatters()
                .AddCustomXmlFormatters()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddApiDocumentation();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app
                .UseMiddleware(this.Configuration)
                .UseMvc();
        }
    }
}
