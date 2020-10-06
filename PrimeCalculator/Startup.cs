using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PrimeCalculator.BackgroundServices.HostedService;
using PrimeCalculator.BackgroundServices.Queue;
using PrimeCalculator.Controllers;
using PrimeCalculator.Dtos;
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

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddMediatR(typeof(Startup));

            services.AddScoped<ICalculationRepository, CalculationRepository>();
            services.AddScoped<IPrimeLinkRepository, PrimeLinkRepository>();

            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
            services.AddHostedService<QueuedHostedService>();

            var connectionStrings = new ConnectionStrings();
            Configuration.Bind("ConnectionStrings", connectionStrings);

            services.AddSingleton(connectionStrings);

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

            services.AddLinks(configure =>
            {
                configure.AddPolicy<LinkDto>("GetCalculationStatePolicy", policy => 
                {
                    policy
                        .RequireSelfLink()
                        .RequireRoutedLink("location", nameof(PrimeController.GetCalculationState), x => new { number = x.Id });
                });
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

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
