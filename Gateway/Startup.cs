using Ocelot.Middleware;
using Ocelot.DependencyInjection;

namespace Gateway
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOcelot(Configuration);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) 
            { 
                app.UseDeveloperExceptionPage(); 
            }

            app.UseRouting(); 
            app.UseOcelot(); 
            app.UseEndpoints(endpoints => 
            { 
                endpoints.MapGet("/", async context => { await context.Response.WriteAsync("Hello World!"); }); 
            });
        }
    }
}