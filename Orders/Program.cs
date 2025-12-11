using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Orders.Data;
using Orders.Services;
using Scalar.AspNetCore;
using Shared.Extensions;

var builder = WebApplication.CreateBuilder(args);

// DbContext
builder.AddNpgsqlDbContext<ApplicationDbContext>("orders");
builder.Services.AddServiceDiscovery();
// HttpClient for Catalog service
builder.Services.AddHttpClient("catalog", client =>
{
    client.BaseAddress = new Uri("https://catalog-service");
}).AddServiceDiscovery();

// JWT Authentication
builder.Services.AddJwtAuthentication(builder.Configuration);

// Controllers
builder.Services.AddControllers();

// Swagger / OpenAPI
builder.Services.AddOpenApi();

// Register your services
builder.Services.AddScoped<IOrderService, OrderService>();

builder.Configuration.AddUserSecrets<Program>();

// API Versioning
// Add API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(2, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

var app = builder.Build();

// Scalar OpenAPI
app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options
        .WithTitle("Orders API")
        .AddDocument("v1", "V1 - Minimal API", "/openapi/v1.json", isDefault: true)
        .AddDocument("v2", "V2 - Controllers", "/openapi/v2.json");
});

// Auto-migrate database in development
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
