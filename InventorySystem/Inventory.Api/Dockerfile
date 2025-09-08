FROM mcr.microsoft.com/dotnet/aspnet:9.0-bookworm-slim AS base
WORKDIR /app
EXPOSE 5148
ENV ASPNETCORE_URLS=http://0.0.0.0:5148
ENV URLS=http://0.0.0.0:5148

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY InventorySystem.sln .
COPY Inventory.Api/Inventory.Api.csproj Inventory.Api/
COPY Inventory.Application/Inventory.Application.csproj Inventory.Application/
COPY Inventory.Infrastructure/Inventory.Infrastructure.csproj Inventory.Infrastructure/
COPY Inventory.Domain/Inventory.Domain.csproj Inventory.Domain/
RUN dotnet restore Inventory.Api/Inventory.Api.csproj

COPY . .
RUN dotnet publish Inventory.Api/Inventory.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
# Extra safety: pass --urls at runtime
ENTRYPOINT ["dotnet", "Inventory.Api.dll", "--urls", "http://0.0.0.0:5148"]
