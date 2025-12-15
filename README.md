# eCommerce Platform

A modern microservices-based eCommerce platform built with .NET and TypeScript, featuring service-to-service communication, containerized deployment, and comprehensive testing.

## 📋 Table of Contents

- [Project Architecture](#project-architecture)
- [Tech Stack](#tech-stack)
- [Services Overview](#services-overview)
- [Getting Started](#getting-started)
- [Installation](#installation)
- [Running the Application](#running-the-application)
- [Project Structure](#project-structure)
- [Testing](#testing)
- [Deployment](#deployment)
- [Contributing](#contributing)

## 🏗️ Project Architecture

This project follows a **microservices architecture** pattern with the following components:

- **API Gateway**: Central entry point for all client requests, routing to appropriate services
- **Identity Service**: User authentication and authorization management
- **Catalog Service**: Product catalog and inventory management
- **Orders Service**: Order processing and management
- **Notifications Service**: Order and user notifications
- **Worker Service**: Background job processing
- **Frontend**: React/TypeScript web application

All services are containerized with Docker and can be orchestrated together using Docker Compose.

## 💻 Tech Stack

### Backend
- **Framework**: .NET (ASP.NET Core)
- **Language**: C#
- **Architecture**: Microservices
- **Communication**: HTTP/gRPC (API Gateway pattern)
- **Container**: Docker

### Frontend
- **Framework**: React
- **Language**: TypeScript
- **Build Tool**: Modern JavaScript tooling (Webpack/Vite)

### Infrastructure
- **Containerization**: Docker & Docker Compose
- **CI/CD**: GitHub Actions (`.github/workflows`)
- **Load Testing**: k6

### Testing
- **Framework**: .NET testing framework (xUnit/NUnit)

## 🎯 Services Overview

### API Gateway (`/ApiGateway`)
- Routes requests to appropriate microservices
- Handles API versioning and routing
- Entry point for frontend applications

### Identity Service (`/Identity`)
- User authentication and authorization
- JWT token generation and validation
- User role management

### Catalog Service (`/Catalog`)
- Product management
- Inventory tracking
- Product search and filtering

### Orders Service (`/Orders`)
- Order creation and management
- Order status tracking
- Order history retrieval

### Notifications Service (`/Notifications`)
- Email/notification dispatch
- Event-based notification triggers
- Notification templates

### Worker Service (`/Worker`)
- Background job processing
- Message queue consumer
- Scheduled tasks

### Frontend (`/Frontend/proyecto`)
- React TypeScript application
- User interface for browsing products
- Shopping cart and checkout flow

## 🚀 Getting Started

### Prerequisites

- **.NET SDK**: 8.0 or higher
- **Docker** and **Docker Compose**
- **Node.js**: 18 or higher (for frontend development)
- **Git**

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/surtote/eCommerce.git
   cd eCommerce
   ```

2. **Restore .NET dependencies**
   ```bash
   dotnet restore
   ```

3. **Install frontend dependencies**
   ```bash
   cd Frontend/proyecto
   npm install
   cd ../..
   ```

## 📦 Running the Application

### Using Docker Compose (Recommended)

```bash
docker-compose up -d
```

This will start all services:
- API Gateway: `http://localhost:8000`
- Frontend: `http://localhost:3000`
- Additional services will be available internally within the Docker network

### Running Locally

#### Backend Services
```bash
# From the project root
dotnet run --project ApiGateway/ApiGateway.csproj
dotnet run --project Identity/Identity.csproj
dotnet run --project Catalog/Catalog.csproj
dotnet run --project Orders/Orders.csproj
```

#### Frontend
```bash
cd Frontend/proyecto
npm start
```

## 📁 Project Structure

```
eCommerce/
├── ApiGateway/              # API Gateway service
├── Catalog/                 # Catalog microservice
├── Ecommerce.tests/         # Unit and integration tests
├── Frontend/                # React TypeScript frontend
│   └── proyecto/            # Main React application
├── Identity/                # Identity/Auth service
├── Notifications/           # Notification service
├── Orders/                  # Orders microservice
├── ServiceDefaults/         # Shared service configuration
├── Shared/                  # Shared utilities and models
├── Worker/                  # Background worker service
├── .github/workflows/       # CI/CD pipelines
├── docker-compose.yml       # Docker Compose configuration
├── eCommerce.sln            # Visual Studio solution file
├── k6-load-test.js          # Load testing script
└── Directory.Packages.props  # Centralized dependency management
```

## 🧪 Testing

Run unit and integration tests:

```bash
dotnet test Ecommerce.tests/Ecommerce.tests.csproj
```

### Load Testing

Execute load tests using k6:

```bash
k6 run k6-load-test.js
```

## 🐳 Deployment

### Docker Build

Build all services:
```bash
docker-compose build
```

### Environment Configuration

Create a `.env` file in the project root for environment-specific variables (database connections, API keys, etc.)

### Production Deployment

The project includes GitHub Actions workflows for automated CI/CD in `.github/workflows/`. These pipelines handle:
- Code compilation
- Running tests
- Docker image building
- Deployment automation

## 📝 Key Features

- **Microservices Architecture**: Independently deployable services
- **API Gateway Pattern**: Centralized request routing
- **Authentication & Authorization**: Secure user management
- **Order Management**: Full order lifecycle support
- **Real-time Notifications**: Event-driven notification system
- **Background Processing**: Worker services for async tasks
- **Containerization**: Full Docker support for easy deployment
- **Automated Testing**: Comprehensive test coverage
- **Load Testing**: k6 integration for performance validation

## 🤝 Contributing

Contributions are welcome! Please feel free to submit pull requests or open issues for bugs and feature requests.

## 📄 License

This project is available on GitHub. Please check the repository for license information.
