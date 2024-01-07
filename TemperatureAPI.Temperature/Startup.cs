using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TemperatureAPI.Temperature.Data;
using TemperatureAPI.Temperature.Models;

namespace TemperatureAPI.Temperature
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
            var connectionString = 
                Configuration.GetConnectionString("Temperatures") 
                ?? 
                "Data Source=Temperatures.db";
            services.AddControllers();
            services.AddScoped<ITemperatureRepository, TemperatureRepository>();
            services.AddDbContext<TemperatureDbContext>(
                options => options.UseSqlite(connectionString));
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1",
                    new OpenApiInfo {
                        Title = "TemperatureAPI.Temperature", 
                        Version = "v1" 
                    });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TemperatureAPI.Temperature v1"));
            }

            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

    }
}
