using Microsoft.AspNetCore.Mvc;
using PrecipitationSensors;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages().AddRazorPagesOptions(o => 
{ 
    o.Conventions.ConfigureFilter(new IgnoreAntiforgeryTokenAttribute()); 
});

// The project is set up as an asp.net web project and does not initialise
// the SensorService until a page is launched. When launched as a docker container
// this page does not launch automaticaly. Thus this needs to be explicitly
// instantiated.
SensorService sensorService = new();
builder.Services.AddSingleton<SensorService>(sensorService);

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapRazorPages();

app.Run();