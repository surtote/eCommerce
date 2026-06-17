# 🛒 eCommerce — Plataforma de comercio electrónico

Aplicación de eCommerce desarrollada con arquitectura de **microservicios** en .NET 10, con un frontend en Next.js 15 y orquestación completa con Docker y .NET Aspire.

---

## 📐 Arquitectura

```
Frontend (Next.js)
        │
        ▼
   API Gateway  ──── JWT Auth ──── Rate Limiting (Redis)
        │
   ┌────┼─────────────┐
   │    │             │
Identity  Catalog   Orders
   │       │           │
  BD      BD          BD
(PostgreSQL)
        │
   RabbitMQ ──► Notifications (emails)
                Worker (tareas en segundo plano)
```

El proyecto está dividido en servicios independientes, cada uno con su propia base de datos. El API Gateway actúa como punto de entrada único, centraliza la autenticación JWT y aplica rate limiting con Redis.

---

## 🧩 Servicios

| Servicio | Descripción | Puerto |
|---|---|---|
| **API Gateway** | Entrada única. YARP reverse proxy + JWT + rate limiting | 7280 |
| **Identity** | Registro, login, usuarios, roles y generación de tokens JWT | 8080 |
| **Catalog** | Productos, categorías, chats entre usuarios | 8081 |
| **Orders** | Gestión de pedidos con versioning de API (v1) | 8082 |
| **Notifications** | Escucha eventos de RabbitMQ y envía emails | — |
| **Worker** | BackgroundService para tareas en segundo plano | — |
| **Frontend** | Interfaz web con Next.js 15 + React 19 | 3000 |

---

## 🛠️ Stack tecnológico

### Backend
- **ASP.NET Core** (.NET 10) — API REST con controllers y minimal APIs
- **Entity Framework Core** — ORM con migraciones y code-first
- **PostgreSQL** — base de datos relacional (una BD por microservicio)
- **JWT Bearer** — autenticación stateless con roles (Admin, Customer)
- **ASP.NET Core Identity** — gestión de usuarios y roles
- **YARP** — reverse proxy para el API Gateway
- **MassTransit + RabbitMQ** — mensajería asíncrona entre microservicios
- **Redis** — rate limiting distribuido
- **FluentValidation** — validación de requests
- **OpenTelemetry** — observabilidad y telemetría
- **Scalar / OpenAPI** — documentación de la API

### Frontend
- **Next.js 15** + **React 19** + **TypeScript**
- **Tailwind CSS 4** — estilos
- **shadcn/ui** + **Radix UI** — componentes de interfaz
- **NextAuth** — gestión de sesión
- **TanStack Table** — tablas de datos

### Infraestructura
- **Docker** + **Docker Compose** — contenerización
- **.NET Aspire** — orquestación en desarrollo
- **NUnit** + **Moq** — tests unitarios

---

## 🚀 Cómo ejecutar el proyecto

### Requisitos previos
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [Node.js 20+](https://nodejs.org/)

### Con .NET Aspire (recomendado)

```bash
# 1. Clonar el repositorio
git clone https://github.com/surtote/eCommerce.git
cd eCommerce

# 2. Arrancar todo con Aspire (levanta BBDDs, RabbitMQ, Redis y todos los servicios)
dotnet run --project eCommerce/eCommerce.csproj
```

Aspire se encarga automáticamente del orden de arranque y de conectar los servicios entre sí.

### Con Docker Compose (solo Identity + PostgreSQL)

```bash
docker-compose up -d
```

---

## 🔐 Autenticación

El sistema usa **JWT Bearer tokens**. El flujo es:

1. El cliente hace `POST /api/auth/login` con email y contraseña
2. Identity valida las credenciales y devuelve un token JWT
3. El cliente incluye el token en cada petición: `Authorization: Bearer <token>`
4. El API Gateway valida el token antes de reenviar la petición al microservicio

### Roles disponibles
- `Admin` — acceso completo, incluye endpoints de administración
- `Customer` — acceso a compras y perfil propio

### Usuario admin por defecto (desarrollo)
```
Email: admin@admin.com
Password: Test12345.
```

---

## 📡 Endpoints principales

### Auth (público)
```
POST /api/v1/auth/register     — Registro de usuario
POST /api/v1/auth/login        — Login, devuelve JWT
```

### Productos (público)
```
GET  /api/products             — Listado de productos
GET  /api/products/{id}        — Detalle de producto
POST /api/products             — Crear producto (autenticado)
PUT  /api/products/{id}        — Editar producto (autenticado)
DELETE /api/products/{id}      — Eliminar producto (autenticado)
```

### Pedidos (autenticado)
```
GET  /api/v1/orders/my         — Mis pedidos
POST /api/v1/orders            — Crear pedido
PUT  /api/v1/orders/{id}/cancel — Cancelar pedido
```

### Administración (solo Admin)
```
GET  /api/v1/admin/orders      — Todos los pedidos
PUT  /api/v1/admin/orders/{id}/status — Cambiar estado de pedido
GET  /api/v1/admin/users       — Gestión de usuarios
```

La documentación interactiva completa está disponible en `/scalar/v1` cuando el proyecto está en ejecución.

---

## 📨 Mensajería con RabbitMQ

Los servicios se comunican de forma **asíncrona** mediante eventos de integración. Cuando ocurre algo relevante, el servicio publica un evento en RabbitMQ en lugar de llamar directamente al otro servicio.

| Evento | Publicado por | Consumido por |
|---|---|---|
| `OrderCreatedEvent` | Orders | Notifications |
| `UserRegisteredEvent` | Identity | Notifications |
| `OrderCancelledEvent` | Orders | Notifications |

Notifications escucha estos eventos y envía el email correspondiente al usuario.

---

## 🧪 Tests

El proyecto incluye tests unitarios con **NUnit** y **Moq** para los servicios principales.

```bash
dotnet test Ecommerce.tests/Ecommerce.tests.csproj
```

Servicios con cobertura de tests: `ProductService`, `CategoryService`, `UserService`, `ChatService`, `MessageService`.

---

## 📁 Estructura del proyecto

```
eCommerce/
├── ApiGateway/          — Reverse proxy (YARP + JWT + Rate limiting)
├── Identity/            — Usuarios, roles y autenticación JWT
├── Catalog/             — Productos, categorías y chats
├── Orders/              — Gestión de pedidos
├── Notifications/       — Envío de emails con MassTransit
├── Worker/              — Tareas en segundo plano
├── Shared/              — Eventos de integración compartidos
├── ServiceDefaults/     — Configuración común (telemetría, health checks)
├── Ecommerce.tests/     — Tests unitarios
├── Frontend/            — Aplicación Next.js
├── docker-compose.yml   — Contenerización
└── eCommerce/           — AppHost de .NET Aspire
```

---

## 📄 Licencia

Proyecto de desarrollo personal con fines educativos y de portfolio.
