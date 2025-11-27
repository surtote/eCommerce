using Asp.Versioning;
using FluentValidation;
using Identity.Data;
using Identity.Extensions;
using Identity.Features.Auth.V1;
using Identity.Features.Roles.V1;
using Identity.Features.Users.V1;
using Identity.Models;
using Identity.Models.Roles.Request;
using Identity.Models.Users.Request;
using Identity.Repository;
using Identity.Services;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add Aspire Service Defaults (OpenTelemetry, Health Checks, Service Discovery, Resilience)
builder.AddServiceDefaults();


// Configure OpenAPI documents for different versions with JWT Bearer authentication
builder.Services.AddOpenApi("v1", options =>
{
    options.ConfigureDocumentInfo(
        "OrderFlow Identity API V1",
        "v1",
        "Authentication API using Minimal APIs with JWT Bearer authentication");
    options.AddJwtBearerSecurity();
});

builder.Services.AddOpenApi("v2", options =>
{
    options.ConfigureDocumentInfo(
        "OrderFlow Identity API V2",
        "v2",
        "Authentication API using Controllers with JWT Bearer authentication");
    options.AddJwtBearerSecurity();
});
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddMassTransit(config =>
{
    config.UsingRabbitMq((context, cfg) =>
    {
        var configuration = context.GetRequiredService<IConfiguration>();
        var connectionString = configuration.GetConnectionString("messaging");
        cfg.Host(new Uri(connectionString));
        cfg.ConfigureEndpoints(context);
    });
});
// Add API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.AddNpgsqlDbContext<ApplicationDbContext>("eCommerce");

// 🔹 Identity
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthorization();

// 🔹 Servicios
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<Identity.Services.IRolesService, Identity.Services.RoleService>();
builder.Services.AddScoped<Identity.Services.Common.ServiceResult, Identity.Services.Common.ServiceResult>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateRoleRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateRoleRequest>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateUserRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateUserRequestValidator>();

builder.Configuration.AddUserSecrets<Program>();

// ============================================
// JWT BEARER AUTHENTICATION
// ============================================
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// 🔹 Controladores y Swagger
builder.Services.AddControllers();
var app = builder.Build();
// 🔹 CORS para permitir cualquier origen (desarrollo)

// ============================================
// SEED DEVELOPMENT DATA (Database, Roles, Admin User)
// ============================================
if (app.Environment.IsDevelopment())
{
   

    // Map OpenAPI documents - uses document names from AddOpenApi configuration
    app.MapOpenApi();

    // path: scalar
    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("OrderFlow Identity API")
            .AddDocument("v1", "V1 - Minimal API", "/openapi/v1.json", isDefault: true)
            .AddDocument("v2", "V2 - Controllers", "/openapi/v2.json");
    });

    //path: swagger

    app.UseSwaggerUI(options =>
    {

        options.SwaggerEndpoint("/openapi/v1.json", "OrderFlow Identity API V1");
        options.SwaggerEndpoint("/openapi/v2.json", "OrderFlow Identity API V2");
    });

}




app.UseHttpsRedirection();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapRegisterUser();
app.MapLoginUser();
//app.MapAuthGroup();
app.MapControllers();
// si quieres auth
app.MapRoleEndpoints();
// User management endpoints
app.MapUserEndpoints();
app.Run();
