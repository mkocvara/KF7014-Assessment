using Microsoft.AspNetCore.Mvc;
using PrecipitationSensors;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages().AddRazorPagesOptions(o => 
{ 
    o.Conventions.ConfigureFilter(new IgnoreAntiforgeryTokenAttribute()); 
});
builder.Services.AddSingleton<SensorService>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapRazorPages();

app.Run();