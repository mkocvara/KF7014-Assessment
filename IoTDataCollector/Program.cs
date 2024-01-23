using IoTHumidityDataCollector.Connectors;
using Microsoft.EntityFrameworkCore;

Console.WriteLine("Initializing...");


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<HumidityContext>();

builder.Services.AddApiVersioning();

var app = builder.Build();

// Create the database if it is not already created
using (IServiceScope scope = app.Services.CreateScope())
{
    // Delete the database if the app is in a development environment
    // Helps debug and fix development issues
    if (app.Environment.IsDevelopment())
    {
        scope.ServiceProvider.GetService<HumidityContext>().Database.EnsureDeleted();
    }
    scope.ServiceProvider.GetService<HumidityContext>().Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

