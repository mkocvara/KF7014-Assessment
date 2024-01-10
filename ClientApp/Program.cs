using ClientApp.Data;
using EasyNetQ;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddHttpClient("Gateway", client =>
{
    client.BaseAddress = new Uri("https://localhost:7081");
    return;
});

// > Register repositories
builder.Services.AddSingleton<IReadOnlyRepository<PrecipitationMeasurement>, PrecipitationRepository>();
// TODO: OTHER REPOSITORIES HERE>

// > Register event bus
builder.Services.AddSingleton<IBus>(RabbitHutch.CreateBus("host=localhost"));

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