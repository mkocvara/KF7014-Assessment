using Microsoft.AspNetCore.Hosting;
using System;

namespace TemperatureAPI.Sensor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Starting Sensor Service");
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}