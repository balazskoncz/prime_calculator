using System.Linq;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using PrimeCalculator.BackgroundServices.HostedService;
using PrimeCalculator.BackgroundServices.Queue;
using PrimeCalculator.Controllers;
using PrimeCalculator.Dtos;
using PrimeCalculator.HealthChecks;
using PrimeCalculator.Helpers;
using PrimeCalculator.MapperProfiles;
using PrimeCalculator.Repositories;
using PrimeCalculator.Repositories.PrimeLink;
using RiskFirst.Hateoas;

namespace PrimeCalculator
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        readonly string AllowAllPolicy = "AllowAll";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
             services.AddCors(options =>
             {
                 options.AddPolicy(name: AllowAllPolicy,
                     builder =>
                     {
                         builder.AllowAnyOrigin();
                         builder.AllowAnyMethod();
                         builder.AllowAnyHeader();
                         builder.AllowCredentials();
                     });
             });

            services.AddControllers();

            services.AddMediatR(typeof(Startup));

            services.AddScoped<ICalculationRepository, CalculationRepository>();
            services.AddScoped<IPrimeLinkRepository, PrimeLinkRepository>();

            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
            services.AddHostedService<QueuedHostedService>();

            var connectionStrings = new ConnectionStrings();
            Configuration.Bind("ConnectionStrings", connectionStrings);

            var policyConfigurations = new PolicyConfiguration();
            Configuration.Bind("PolicyConfiguration", policyConfigurations);

            services.AddSingleton(connectionStrings);
            services.AddSingleton(policyConfigurations);

            var connectionString = string.Format(Configuration["ConnectionStrings:DatabaseConnection"]);
            services.AddEntityFrameworkNpgsql()
                .AddDbContext<PrimeDbContext>(options => options
                    .UseNpgsql(connectionString, options =>
                    {
                        options.MigrationsAssembly(typeof(PrimeDbContext).Assembly.FullName);
                    }));

            services.AddAutoMapper(configuration =>
            {
                configuration.AddProfile(new CalculationProfile());
                configuration.AddProfile(new PrimeLinkProfile());
            });

            services.AddHealthChecks()
                .AddCheck<ScienceHealthCheck>("Science service host connection")
                .AddCheck<DatabaseHealthChecks>("Database service host connection");

            services.AddLinks(configure =>
            {
                configure.AddPolicy<LinkDto>("GetCalculationStatePolicy", policy => 
                {
                    policy
                        .RequireSelfLink()
                        .RequireRoutedLink("location", nameof(PrimeController.GetCalculationState), x => new { number = x.Id });
                });
                configure.AddPolicy<LinkDto>("GetPrimeLinkStatePolicy", policy =>
                {
                    policy
                        .RequireSelfLink()
                        .RequireRoutedLink("location", nameof(PrimeController.GetPrimeLink), x => new { number = x.Id });
                });
            });

            services.AddOpenApiDocument(settings => 
            {
                settings.Title = "Prime API";
                settings.DocumentName = "v1";
                settings.Version = "v1";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<PrimeDbContext>();
                context.Database.EnsureCreated();
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(AllowAllPolicy);

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                var options = new HealthCheckOptions();
                options.ResponseWriter = async (c, r) => {

                    c.Response.ContentType = "application/json";

                    var result = JsonConvert.SerializeObject(new
                    {
                        status = r.Status.ToString(),
                        errors = r.Entries.Select(e => new { key = e.Key, value = e.Value.Status.ToString() })
                    });

                    await c.Response.WriteAsync(result);
                };

                endpoints.MapHealthChecks("/health", options);
            });

            app.UseOpenApi()
               .UseSwaggerUi3();
        }
    }
}
