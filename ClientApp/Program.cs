using ClientApp.Areas.Identity.Data;
using ClientApp.Data;
using ClientApp.Data.Repositories;
using EasyNetQ;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
var connectionString = Environment.GetEnvironmentVariable("MYSQL_URI") ?? throw new InvalidOperationException("Connection string 'MYSQL_URI' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseMySQL(connectionString));

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();



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

builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings.
    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = false;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
});

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

var app = builder.Build();

using (IServiceScope scope = app.Services.CreateScope())
{
    DatabaseFacade db = scope.ServiceProvider.GetService<ApplicationDbContext>().Database;

    db.Migrate();
    db.EnsureCreated();
}

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

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();