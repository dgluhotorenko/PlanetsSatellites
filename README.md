# PlanetsSatellites

A microservices demo built with **.NET 9**, showcasing inter-service communication patterns (HTTP, gRPC, RabbitMQ), JWT authentication, Docker, and Kubernetes orchestration. Includes a **Blazor WebAssembly** UI (MudBlazor) that visualises the replication flow end-to-end.

## Architecture

```
                          ┌──────────────────────────────────────────────────┐
                          │          Internal Microservices Domain           │
                          │                                                  │
  ┌────────┐   REST API   │  ┌────────────────┐      ┌──────────────┐        │
  │  API   │─────────────►│  │ PlanetService  │──────│  SQL Server  │        │
  │Gateway │              │  │  (port 5000)   │      │  (port 1433) │        │
  │(Ingress│   REST API   │  └──────┬───┬─────┘      └──────────────┘        │
  │ Nginx) │──────┐       │     gRPC│   │Publish                             │
  │        │      │       │         │   ▼                                    │
  │        │  REST│API    │         │  ┌──────────────────┐                  │
  │        │──┐   │       │         │  │ RabbitMQ Message │                  │
  └────────┘  │   │       │         │  │   Bus (5672)     │                  │
              │   │       │         │  └────────┬─────────┘                  │
              │   │       │         │       Subscribe                        │
              ▼   ▼       │         ▼           │                            │
         ┌──────────────┐ │  ┌─────────────────┐│  ┌──────────┐              │
         │ AuthService  │ │  │SatelliteService ◄┘  │ InMemory │              │
         │ (port 7000)  │ │  │  (port 6000)    │───│    DB    │              │
         └──────────────┘ │  └─────────────────┘   └──────────┘              │
              │           │                                                  │
              ▼           └──────────────────────────────────────────────────┘
         ┌──────────┐
         │SQL Server│
         └──────────┘
```

### Services

| Service | Port (host) | Description |
|---|---|---|
| **PlanetService** | 5000 (HTTP), 50051 (gRPC) | CRUD for planets. Publishes events via RabbitMQ and sends sync HTTP notifications. Exposes gRPC endpoint for SatelliteService. |
| **SatelliteService** | 6001 → 6000 in container | Manages satellites linked to planets. Subscribes to RabbitMQ events. Fetches initial planet data via gRPC on startup. Host port is `6001` because Chrome blocks calls to `:6000` as `ERR_UNSAFE_PORT` (reserved for X11). |
| **AuthService** | 7000 | User registration and login. Issues JWT tokens used by other services for authorization. |
| **PlanetsSatellites.Web** | 5257 (dev server) | Blazor WebAssembly UI (MudBlazor). Login / register, planet CRUD, satellite CRUD, and a live "Replication" column that times RabbitMQ propagation from PlanetService to SatelliteService. |

### Communication Patterns

- **Synchronous HTTP** — PlanetService notifies SatelliteService when a new planet is created
- **Synchronous gRPC** — SatelliteService fetches all planets from PlanetService on startup
- **Asynchronous RabbitMQ** — PlanetService publishes `Planet_Published` events; SatelliteService subscribes and creates local copies

## Tech Stack

- .NET 9 (LTS), ASP.NET Core Web API
- Entity Framework Core (SQL Server + InMemory)
- gRPC (Protobuf)
- RabbitMQ (async messaging)
- ASP.NET Core Identity + JWT Bearer authentication
- Docker & Kubernetes
- xUnit + Moq (testing)

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) (or .NET 10 SDK — it can build .NET 9 projects)
- [Docker Desktop](https://docs.docker.com/desktop/) (for Docker Compose or Kubernetes)

## Quick Start (Docker Compose)

The easiest way to run everything locally:

```bash
# 1. Clone the repository
git clone https://github.com/dgluhotorenko/PlanetsSatellites.git
cd PlanetsSatellites

# 2. Start all services (builds images, starts MSSQL, RabbitMQ, and all 3 services)
docker compose up --build

# 3. Wait until all services are healthy (about 30-60 seconds for MSSQL to initialize)
```

That's it! All services are running:
- **AuthService**: http://localhost:7000
- **PlanetService**: http://localhost:5000
- **SatelliteService**: http://localhost:6001 *(container still listens on 6000; host maps it to 6001 because Chrome blocks `:6000`)*
- **RabbitMQ Management UI**: http://localhost:15672 (guest/guest)

The Blazor UI is **not** part of `docker compose up` — see [Running the UI](#running-the-ui) below.

### Try the API

**Step 1: Register a user**
```bash
curl -X POST http://localhost:7000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"email": "test@example.com", "password": "P@ssw0rd123"}'
```

**Step 2: Login and get a JWT token**
```bash
curl -X POST http://localhost:7000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email": "test@example.com", "password": "P@ssw0rd123"}'
```
Copy the `token` value from the response.

**Step 3: Get all planets (requires token)**
```bash
curl http://localhost:5000/api/planet \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```
Returns the 5 seeded planets (Mercury, Venus, Earth, Mars, Jupiter).

**Step 4: Create a new planet**
```bash
curl -X POST http://localhost:5000/api/planet \
  -H "Authorization: Bearer YOUR_TOKEN_HERE" \
  -H "Content-Type: application/json" \
  -d '{"name": "Saturn", "mass": 95.16, "radius": 58232.0}'
```
This triggers:
1. Synchronous HTTP notification to SatelliteService
2. Asynchronous RabbitMQ event (`Planet_Published`)
3. SatelliteService receives the event and creates a local copy

**Step 5: Verify planet sync on SatelliteService**
```bash
curl http://localhost:6001/api/s/planet \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```
You should see all planets including the newly created Saturn.

**Step 6: Create a satellite for a planet**
```bash
curl -X POST http://localhost:6001/api/s/planets/1/satellite \
  -H "Authorization: Bearer YOUR_TOKEN_HERE" \
  -H "Content-Type: application/json" \
  -d '{"name": "Titan", "type": "Natural"}'
```

**Step 7: Get satellites for a planet**
```bash
curl http://localhost:6001/api/s/planets/1/satellite \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

### Stop everything

```bash
docker compose down        # stop and remove containers
docker compose down -v     # also remove the MSSQL data volume
```

## Running the UI

The Blazor WebAssembly client is a separate project. It is **not** started by `docker compose`
because its API base URLs are baked into `wwwroot/appsettings.json` at build time and are set
for local-host usage. Run it in parallel to the backend:

```bash
# 1. Start the backend (from the repo root)
docker compose up --build

# 2. In a second terminal — start the UI dev server
dotnet run --project PlanetsSatellites.Web
```

Open http://localhost:5257. You can:
- **Register** a new account (persisted in `AuthDb` on SQL Server).
- **Log in** — a JWT is stored in `localStorage`; every subsequent API call adds `Authorization: Bearer …` automatically.
- **Browse planets** — the table shows a live **Replication** column that times RabbitMQ propagation to SatelliteService for every planet you create.
- **Open a planet** — add satellites (Natural / Artificial) and see them stored in SatelliteService.
- Each page has an expandable **"Under the hood"** panel explaining which services, protocols and storage are touched by that view.

The UI reads API endpoints from `PlanetsSatellites.Web/wwwroot/appsettings.json`:

```json
{
  "Apis": {
    "Auth":      "http://localhost:7000",
    "Planet":    "http://localhost:5000",
    "Satellite": "http://localhost:6001"
  }
}
```

CORS is allow-listed for `http://localhost:5257` on all three backend services (override with the
`Cors:AllowedOrigins` configuration key / env var).

> **Chrome note:** the satellite host port is **6001**, not 6000 — Chrome blocks `:6000` as
> `ERR_UNSAFE_PORT` (reserved for X11). The container still listens on 6000 internally; only the
> host mapping changed.

## Running Tests

```bash
dotnet test
```

Runs 38 tests across 3 test projects:
- `PlanetService.Tests` — repository, mapper, and controller unit tests
- `SatelliteService.Tests` — repository, mapper, and event processor tests
- `AuthService.Tests` — auth controller tests (register, login, token generation)

## Running Locally (without Docker)

1. Start a SQL Server instance on `localhost:1433` (for AuthService)
2. Start RabbitMQ on `localhost:5672`
3. Run each service:

```bash
dotnet run --project AuthService
dotnet run --project PlanetService
dotnet run --project SatelliteService
```

PlanetService uses an in-memory database in development mode, so no SQL Server is needed for it.
SatelliteService always uses an in-memory database.
AuthService requires SQL Server for ASP.NET Core Identity.

The UI can be started the same way:

```bash
dotnet run --project PlanetsSatellites.Web
```

then browse http://localhost:5257.

## Kubernetes Deployment

For production-like deployment using Kubernetes:

```bash
cd K8S

# Create persistent storage for SQL Server
kubectl apply -f local-pvc.yaml

# Create secrets
kubectl create secret generic mssql --from-literal=SA_PASSWORD="pa55w0rd!"
kubectl create secret generic auth-jwt-secret \
  --from-literal=Jwt__Key="YourProductionSecretKey_MinLength32Chars!" \
  --from-literal=Jwt__Issuer="PlanetsSatellitesAuth" \
  --from-literal=Jwt__Audience="PlanetsSatellitesUsers"

# Deploy infrastructure
kubectl apply -f mssql-planet-depl.yaml
kubectl apply -f rabbitmq-depl.yaml

# Deploy NGINX Ingress Controller
kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.12.0-beta.0/deploy/static/provider/cloud/deploy.yaml

# Deploy services
kubectl apply -f auth-depl.yaml
kubectl apply -f planet-depl.yaml
kubectl apply -f satellite-depl.yaml
kubectl apply -f ingress-srv.yaml
```

Add to your hosts file:
```
127.0.0.1 planetssatellites.com
```

### Accessing the API

The Ingress controller exposes a LoadBalancer on port 80. If port 80 is free on your machine:

```
http://planetssatellites.com/api/planet
```

If port 80 is already in use (e.g., by IIS), use `kubectl port-forward` instead:

```bash
kubectl port-forward -n ingress-nginx svc/ingress-nginx-controller 8080:80
```

Then access the API at `http://planetssatellites.com:8080/api/planet`, etc.

### Testing the K8S deployment

```bash
# 1. Register
curl -X POST http://planetssatellites.com:8080/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"email": "test@example.com", "password": "P@ssw0rd123"}'

# 2. Login (copy the token from the response)
curl -X POST http://planetssatellites.com:8080/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email": "test@example.com", "password": "P@ssw0rd123"}'

# 3. Get planets (replace YOUR_TOKEN_HERE)
curl http://planetssatellites.com:8080/api/planet \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

> Replace `:8080` with `:80` (or omit the port) if you're not using port-forward.

### Cleanup K8S

```bash
kubectl delete -f K8S/
kubectl delete secret mssql auth-jwt-secret
```

## API Reference

### AuthService

| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/auth/register` | Register a new user |
| POST | `/api/auth/login` | Login and receive a JWT token |

### PlanetService (requires JWT token)

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/planet` | Get all planets |
| GET | `/api/planet/{id}` | Get a planet by ID |
| POST | `/api/planet` | Create a new planet |

### SatelliteService (requires JWT token)

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/s/planet` | Get all planets (local copies) |
| GET | `/api/s/planets/{planetId}/satellite` | Get all satellites for a planet |
| GET | `/api/s/planets/{planetId}/satellite/{satelliteId}` | Get a specific satellite |
| POST | `/api/s/planets/{planetId}/satellite` | Create a satellite for a planet |

All services expose a `/health` endpoint for health checks.

## Project Structure

```
PlanetsSatellites/
├── PlanetService/              # Planet management microservice
│   ├── Controllers/            # REST API controllers
│   ├── Data/                   # EF Core context, repository, seeder
│   ├── DTOs/                   # Data transfer objects
│   ├── Mappers/                # Manual DTO <-> Model mappers
│   ├── Models/                 # Domain models
│   ├── AsyncDataServices/      # RabbitMQ publisher
│   ├── SyncDataServices/       # HTTP client + gRPC server
│   └── Protos/                 # Protobuf definitions
├── SatelliteService/           # Satellite management microservice
│   ├── Controllers/            # REST API controllers
│   ├── Data/                   # EF Core context, repository, seed
│   ├── DTOs/                   # Data transfer objects
│   ├── Mappers/                # Manual DTO <-> Model mappers
│   ├── Models/                 # Domain models
│   ├── AsyncDataServices/      # RabbitMQ subscriber
│   ├── SyncDataServices/       # gRPC client
│   ├── EventProcessing/        # Message bus event handler
│   └── Protos/                 # Protobuf definitions
├── AuthService/                # Authentication microservice
│   ├── Controllers/            # Auth endpoints (register, login)
│   ├── Data/                   # Identity DbContext
│   └── Models/                 # User and auth models
├── PlanetsSatellites.Web/      # Blazor WebAssembly UI (MudBlazor)
│   ├── Components/             # Reusable UI components (TechInfoPanel)
│   ├── Layout/                 # Main layout + nav menu
│   ├── Models/                 # DTOs mirroring backend contracts
│   ├── Pages/                  # Home / Register / Login / Planets / PlanetDetails + dialogs
│   ├── Services/               # API clients, JWT auth state provider, Bearer handler
│   └── wwwroot/                # Static assets + appsettings.json with API URLs
├── PlanetService.Tests/        # Unit tests for PlanetService
├── SatelliteService.Tests/     # Unit tests for SatelliteService
├── AuthService.Tests/          # Unit tests for AuthService
├── K8S/                        # Kubernetes deployment manifests
├── docker-compose.yml          # Docker Compose for local development
├── BACKLOG.md                  # Prioritised improvements (senior/staff-level gaps)
└── PlanetsSatellites.sln       # Solution file
```

## Docker Images

Pre-built images for the three backend services are available on Docker Hub:
- [dgluhotorenko/planetservice](https://hub.docker.com/r/dgluhotorenko/planetservice)
- [dgluhotorenko/satelliteservice](https://hub.docker.com/r/dgluhotorenko/satelliteservice)
- [dgluhotorenko/authservice](https://hub.docker.com/r/dgluhotorenko/authservice)

The Blazor UI is not published as a Docker image — it's a static-file bundle and the API URLs are
baked into the WASM build. Run it locally with `dotnet run --project PlanetsSatellites.Web`.
