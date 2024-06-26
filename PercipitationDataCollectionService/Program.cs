using EasyNetQ;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PrecipitationService.Db;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<PrecipitationDb>(options =>
{
    options.UseSqlite("Data Source=precipitation.db");
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "PrecipitationService",
        Version = "v1"
    });
});

// > Register event bus
builder.Services.AddSingleton<IBus>(RabbitHutch.CreateBus($"host={Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost"}"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();
