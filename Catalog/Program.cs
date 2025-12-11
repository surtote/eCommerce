using Catalog.Data;
using Catalog.Repositories;
using Catalog.Services;
using Scalar.AspNetCore;
using Shared.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Aspire defaults
builder.AddServiceDefaults();

// DbContext
builder.AddNpgsqlDbContext<ApplicationDbContext>("catalog");
// JWT Authentication
builder.Services.AddJwtAuthentication(builder.Configuration);
// Repositorios y servicios
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IChatRepository, ChatRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Configuration.AddUserSecrets<Program>();

// Controladores y OpenAPI
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

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

// Pipeline HTTP
app.UseHttpsRedirection();
app.UseCors("FrontendPolicy");
app.UseAuthorization();
app.MapControllers();

app.Run();
