using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using RzR.ResultMessage.Web;
using RzR.ResultMessage.Web.Abstractions;
using TestWebApi.ProblemDetails;
using TestWebApi.Services;

namespace TestWebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TestWebApi", Version = "v1" });
            });

            services.AddScoped<WeatherService>();

            // Branded ProblemDetails defaults applied globally to every AsProblemDetails(...) call.
            // HttpContextAccessor uses a static AsyncLocal, so a manual instance still
            // observes the current request once UseRouting/middleware runs.
            services.AddHttpContextAccessor();
            services.AddProblemDetailsResultFactory(
                new BrandedProblemDetailsResultFactory(new HttpContextAccessor()));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TestWebApi v1"));
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}