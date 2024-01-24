using ClientApp.Data;
using ClientApp.Data.Repositories;
using EasyNetQ;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddHttpClient("Gateway", client =>
{
    // client.BaseAddress = new Uri("https://localhost:7081");
    client.BaseAddress = new Uri(Environment.GetEnvironmentVariable("GATEWAY_URI")??"https://localhost:7081");
    return;
});

// > Register repositories
builder.Services.AddSingleton<IReadOnlyRepository<PrecipitationMeasurement>, PrecipitationRepository>();
builder.Services.AddSingleton<IReadOnlyRepository<TemperatureMeasurement>, TemperatureRepository>();
builder.Services.AddSingleton<IReadOnlyRepository<HumidityMeasurement>, HumidityRepository>();
// TODO: OTHER REPOSITORIES HERE
builder.Services.AddSingleton<IReadOnlyRepository<WeatherMeasurement>, AggregateRepository>();

// > Register event bus
builder.Services.AddSingleton<IBus>(RabbitHutch.CreateBus($"host={Environment.GetEnvironmentVariable("RABBITMQ_HOST")??"localhost"}"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();