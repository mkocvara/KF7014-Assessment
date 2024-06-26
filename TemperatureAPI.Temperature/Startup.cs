﻿using EasyNetQ;
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

            services.AddSingleton<IBus>(RabbitHutch.CreateBus($"host={Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost"}"));

            
        }

        public async void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TemperatureAPI.Temperature v1"));
            }

            app.UseRouting();
            app.UseAuthorization();

            // Create the database if it is not already created
            using (IServiceScope scope = app.ApplicationServices.CreateScope())
            {
                scope.ServiceProvider.GetService<TemperatureDbContext>()?.Database.EnsureCreated();
            }

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

    }
}
