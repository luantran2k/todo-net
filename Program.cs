using System.Reflection;
var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseKestrel()
               .UseConfiguration(builder.Configuration);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// 🔥 Auto-register services in the 'Services' namespace
var assembly = Assembly.GetExecutingAssembly();

var serviceTypes = assembly
    .GetTypes()
    .Where(t => t.IsClass && !t.IsAbstract && t.Namespace == "MyApiProject.Services");

foreach (var implementationType in serviceTypes)
{
    var interfaceType = implementationType.GetInterface($"I{implementationType.Name}");
    if (interfaceType != null)
    {
        builder.Services.AddScoped(interfaceType, implementationType);
    }
    else
    {
        // Optional: register class directly if no interface
        builder.Services.AddScoped(implementationType);
    }
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/", () =>
{
    return "Hello api net-todo";
}).WithName("HEllo page");

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.MapControllers();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

