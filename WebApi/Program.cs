using DBEntities;
using Service1;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddTransient<IService, Service>(    _ => new Service(connectionString));
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapGet("/api/search", (IService service, string type, string name) =>
{
    if (string.IsNullOrWhiteSpace(type) || string.IsNullOrWhiteSpace(name))
    {
        return Results.BadRequest("Type and name parameters are required.");
    }

    try
    {
        if (type.Equals("country", StringComparison.OrdinalIgnoreCase))
        {
            var currencies = service.SearchByCountry(name);
            return Results.Ok(new
            {
                CountryName = name,
                Currencies = currencies.Select(c => new { c.Name, c.Rate }).ToList()
            });
        }
        else if (type.Equals("currency", StringComparison.OrdinalIgnoreCase))
        {
            var countries = service.SearchByCurrency(name);
            return Results.Ok(new
            {
                CurrencyName = name,
                Countries = countries.Select(c => new { Name = c }).ToList()
            });
        }
        else
        {
            return Results.BadRequest("Invalid type. Use 'country' or 'currency'.");
        }
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.MapPost("/api/currency", (IService logic, CurrencyRequestDTO request) =>
{
    if (string.IsNullOrWhiteSpace(request.CurrencyName) || request.RatetoUSD <= 0 || request.Countries == null || request.Countries.Count == 0)
    {
        return Results.BadRequest("Invalid request data: Name, rate, and countries are required.");
    }

    try
    {
        if (logic.CurrencyExists(request.CurrencyName))
        {
            bool updated = logic.UpdateCurrency(request);
            if (!updated)
            {
                return Results.BadRequest("One or more countries do not exist.");
            }
            return Results.Ok("Currency updated successfully.");
        }
        else
        {
            bool created = logic.CreateCurrency(request);
            if (!created)
            {
                return Results.BadRequest("One or more countries do not exist.");
            }
            return Results.Ok("Currency created successfully.");
        }
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});


app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}