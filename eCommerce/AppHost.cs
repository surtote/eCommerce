using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Postgres;

var builder = DistributedApplication.CreateBuilder(args);

// 🗄️ 1️⃣ PostgreSQL persistente
var postgres = builder.AddPostgres("postgres")
    .WithLifetime(ContainerLifetime.Persistent);

// 🗃️ 2️⃣ Base de datos
var db = postgres.AddDatabase("eCommerce");

// 🔐 3️⃣ API Identity con JWT + DB
var identity = builder.AddProject<Projects.Identity>("identity")
    .WithReference(db)
    .WaitFor(db)
    .WithEnvironment("Jwt__Key", "OzA7O+eSUMejShU35IUD2qlM6ckcfxsMGqg39UeTNz0geOQw3sQP3VEgJomrxBn2")
    .WithEnvironment("Jwt__Issuer", "https://localhost:5001")
    .WithEnvironment("Jwt__Audience", "https://localhost:5001")
    .WithEnvironment("Jwt__ExpireHours", "2");

// 💻 4️⃣ Frontend (Next.js)
var frontend = builder.AddNpmApp("frontend", "../Re-Sports/Frontend/proyecto")
    .WithHttpEndpoint(env: "DEV", targetPort: 3000)
    .WithEnvironment("NEXT_PUBLIC_API_URL", identity.GetEndpoint("http"))
    .WithReference(identity);

// 🚀 5️⃣ Ejecutar Aspire
builder.Build().Run();
