using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Postgres;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

// ========== PostgreSQL para Identity ==========
var postgres = builder.AddPostgres("postgres")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithPgAdmin()
    .WithDataVolume("ecommerce_postgres_data")
    .WithEnvironment("POSTGRES_USER", "postgres");

var db = postgres.AddDatabase("eCommerce");
var dbCatalog = postgres.AddDatabase("catalog");

// ========== Redis (único y compartido) ==========
var redis = builder.AddRedis("cache")
    .WithDataVolume("orderflow-redis-data")
    .WithHostPort(6379)
    .WithLifetime(ContainerLifetime.Persistent);
// RabbitMQ - Message broker for reliable event-driven communication
var rabbitmq = builder.AddRabbitMQ("messaging")
    .WithDataVolume("orderflow-rabbitmq-data")
    .WithManagementPlugin()
    .WithLifetime(ContainerLifetime.Persistent);

// MailDev - Local SMTP server for development (Web UI on 1080, SMTP on 1025)
var maildev = builder.AddContainer("maildev", "maildev/maildev")
    .WithHttpEndpoint(port: 1080, targetPort: 1080, name: "web")
    .WithEndpoint(port: 1025, targetPort: 1025, name: "smtp")
    .WithLifetime(ContainerLifetime.Persistent);
// ========== Servicios ==========
var identityService = builder.AddProject<Projects.Identity>("identity")
    .WithReference(db)
    .WaitFor(db)
    .WaitFor(rabbitmq)
    .WithReference(rabbitmq);

var catalogService = builder.AddProject<Projects.Catalog>("catalog-service")
    .WithReference(dbCatalog)
    .WaitFor(dbCatalog);
// Notifications Worker - Listens to RabbitMQ events and sends emails
var notificationsService = builder.AddProject<Projects.Notifications>("notifications")
    .WithReference(rabbitmq)
    .WithEnvironment("Email__SmtpHost", maildev.GetEndpoint("smtp").Property(EndpointProperty.Host))
    .WithEnvironment("Email__SmtpPort", maildev.GetEndpoint("smtp").Property(EndpointProperty.Port))
    .WaitFor(rabbitmq);
// ========== API Gateway (solo uno) ==========
var apiGateway = builder.AddProject<Projects.ApiGateway>("apigateway")
    .WithReference(redis)
    .WithReference(identityService)
    .WithReference(catalogService)
    .WaitFor(identityService)
    .WaitFor(catalogService);

// ========== Frontend ==========
var frontend = builder.AddNpmApp("frontend", "../Frontend/proyecto")
    .WithReference(apiGateway)
    .WithHttpEndpoint(env: "DEV", targetPort: 3000)
    .PublishAsDockerFile();

// Ejecutar Aspire
builder.Build().Run();
