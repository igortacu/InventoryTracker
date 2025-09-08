# InventorySystem

End-to-end inventory tracker:
- **API**: .NET 9 Minimal API (CQRS with MediatR + EF Core 9, SQL Server)
- **DB**: SQL Server 2022 in Docker
- **UI**: Blazor WebAssembly (Items CRUD)

## Stack
- .NET 9, EF Core 9, MediatR 13
- SQL Server 2022 (Linux container)
- Docker / Docker Compose

## Project layout
Inventory.Domain/
Inventory.Application/ # CQRS (Commands/Queries) via MediatR
Inventory.Infrastructure/ # EF Core DbContext, migrations
Inventory.Api/ # Minimal API (runs on :5148), Swagger, CORS, health checks
Inventory.Web/ # Blazor WASM client (dev server :5286)
docker-compose.yml

## Prerequisites
- Docker & Docker Compose
- .NET 9 SDK

- ## Quick start (everything with Docker + local UI)

1) **Bring up SQL + API**
```bash
cd InventorySystem
docker compose up -d
docker compose ps
```


2) **Create/update DB schema (run once from host)**
   export ConnectionStrings__Default='Server=localhost,1433;Database=InventoryDb;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=true;'
dotnet ef database update -p Inventory.Infrastructure -s Inventory.Api

The API (in Docker) uses Server=sql,1433 via compose env; the CLI (from host) uses localhost,1433.
3) **Verify API**
```bash
curl -i http://localhost:5148/
curl -i http://localhost:5148/healthz/db
curl -i -H "Origin: http://localhost:5286" \
```
4) **Run the Blazor WASM**
```bash
dotnet watch run --project Inventory.Web
```
