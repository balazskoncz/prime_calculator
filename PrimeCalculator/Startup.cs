using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PrimeCalculator.MapperProfiles;
using PrimeCalculator.Repositories;
using PrimeCalculator.Repositories.PrimeLink;

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

            var connectionString = string.Format(Configuration["ConnectionStrings:DefaultConnection"]);
            services.AddEntityFrameworkNpgsql()
                .AddDbContext<PrimeDbContext>(options => options
                    .UseNpgsql(connectionString, options =>
                    {
                        options.MigrationsAssembly(typeof(PrimeDbContext).Assembly.FullName);
                    }));

            var autoMapperConfiguration = new MapperConfiguration(configuration => 
            {
                configuration.AddProfile(new CalculationProfile());
            });

            IMapper mapper = autoMapperConfiguration.CreateMapper();

            services.AddAutoMapper(configuration =>
            {
                configuration.AddProfile(new CalculationProfile());
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
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
