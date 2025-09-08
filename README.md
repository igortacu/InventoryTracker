# InventorySystem

A small, end-to-end **inventory tracker**:

- **API**: .NET 9 Minimal API (CQRS with **MediatR**, **EF Core 9**)
- **DB**: SQL Server 2022 in **Docker**
- **UI**: **Blazor WebAssembly** (Items CRUD)

---

## Features

- Items **CRUD** (create, list with search + paging, update, delete)
- **Adjust quantity** endpoint (+/−)
- **Swagger/OpenAPI**, **health checks**, **dev CORS**
- Dockerized **API + SQL** (one `docker compose up`)
- Blazor WASM client with a simple, clean Items page

---

## Tech Stack

- **.NET 9**, **EF Core 9**, **MediatR 13**
- **SQL Server 2022** (Linux container)
- **Blazor WebAssembly**
- **Docker / Docker Compose**

---

## Project Layout

InventorySystem/
├── Inventory.Domain/
├── Inventory.Application/
│   ├── Items/
│   │   ├── Commands/
│   │   └── Queries/
│   └── IInventoryDbContext.cs
├── Inventory.Infrastructure/
│   ├── InventoryDbContext.cs
│   └── Migrations/
├── Inventory.Api/
│   ├── Program.cs
│   ├── Properties/launchSettings.json
│   └── Dockerfile
├── Inventory.Web/
│   ├── Pages/
│   │   ├── Items.razor
│   │   └── Home.razor
│   ├── Services/ItemsClient.cs
│   └── wwwroot/appsettings.json 
└── docker-compose.yml




**Ports**

- API: `http://localhost:5148`
- Blazor dev: `http://localhost:5286`
- SQL Server: `localhost:1433`

## Quick Start

> **Prerequisites**  
> - Docker & Docker Compose  
> - .NET 9 SDK  
> - (CLI) EF tools: `dotnet tool install -g dotnet-ef`

### 1) Start SQL + API (Docker)

```bash
cd InventorySystem
docker compose up -d
docker compose ps
```

### 2) Apply the database schema (migrations)
```bash
export ConnectionStrings__Default='Server=localhost,1433;Database=InventoryDb;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=true;'
dotnet ef database update -p Inventory.Infrastructure -s Inventory.Api
```


### 3) Verify API is healthy

```bash
curl -i http://localhost:5148/
curl -i http://localhost:5148/healthz/db
curl -i -H "Origin: http://localhost:5286" "http://localhost:5148/items?page=1&pageSize=20"
```


### 4) Run the Blazor WASM
```bash
dotnet run --project Inventory.Web
```


### Curl Cheatsheet

```bash
# List
curl "http://localhost:5148/items?page=1&pageSize=20&search="

# Create
curl -i -H "Content-Type: application/json" \
  -d '{"sku":"X1","name":"Sample","quantity":1,"location":"MAIN","minStock":0}' \
  http://localhost:5148/items

# Get by id
curl "http://localhost:5148/items/{id}"

# Update
curl -i -X PUT -H "Content-Type: application/json" \
  -d '{"name":"Updated","location":"A1","minStock":2}' \
  http://localhost:5148/items/{id}

# Adjust (+5)
curl -i -X POST "http://localhost:5148/items/{id}/adjust?delta=5"

# Delete
curl -i -X DELETE "http://localhost:5148/items/{id}"
```


### Useful Docker commands
```bash
docker ps                       # running containers
docker compose ps               # project services
docker logs -f inv-api          # API logs
docker logs -f inv-sql          # SQL logs
docker exec -it inv-api sh      # shell into API
docker exec -it inv-sql bash    # shell into SQL (if bash available)
docker compose down             # stop containers
docker compose down -v          # stop + remove volumes (wipe DB)
```


### EF Core Migrations
```bash
# add a migration
dotnet ef migrations add MeaningfulName \
  -p Inventory.Infrastructure -s Inventory.Api

# apply to DB
dotnet ef database update \
  -p Inventory.Infrastructure -s Inventory.Api
```

