using Microsoft.OpenApi.Models;
using TemperatureAPI.Sensor.Models;

namespace TemperatureAPI.Sensor
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
            services.AddControllers();
            services.AddScoped<ISensorRepository, SensorRepository>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TemperatureAPI.Sensor", Version = "v1" });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime hostApplicationLifetime)
        {
            hostApplicationLifetime.ApplicationStarted.Register(() =>
            {
                // Code to run when the application has started
                Console.WriteLine("Application started!");
                new HttpClient().GetAsync("https://localhost:7081/Sensor");
            });

            hostApplicationLifetime.ApplicationStopping.Register(() =>
            {
                // Code to run when the application is stopping
                Console.WriteLine("Application stopping...");
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();

                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TemperatureAPI.Sensor v1"));
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
