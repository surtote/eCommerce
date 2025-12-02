
using Microsoft.EntityFrameworkCore;
using Orders.Data;
using Scalar.AspNetCore;
using Shared.Extensions;

var builder = WebApplication.CreateBuilder(args);
// DbContext
builder.AddNpgsqlDbContext<ApplicationDbContext>("orders");

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();
// Add HttpClient for Catalog service
builder.Services.AddHttpClient("catalog", client =>
{
    client.BaseAddress = new Uri("https+http://catalog-service");
});

// JWT Authentication (shared across all microservices)
builder.Services.AddJwtAuthentication(builder.Configuration);

// Scalar OpenAPI
app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options
        .WithTitle("Catalog API")
        .AddDocument("v1", "V1 - Minimal API", "/openapi/v1.json", isDefault: true)
        .AddDocument("v2", "V2 - Controllers", "/openapi/v2.json");
});
// Auto-migrate database in development
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();

    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
