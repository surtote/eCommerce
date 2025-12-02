
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Scalar OpenAPI
app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options
        .WithTitle("Catalog API")
        .AddDocument("v1", "V1 - Minimal API", "/openapi/v1.json", isDefault: true)
        .AddDocument("v2", "V2 - Controllers", "/openapi/v2.json");
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
