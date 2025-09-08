FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY InventorySystem.sln .
COPY Inventory.Api/Inventory.Api.csproj Inventory.Api/
COPY Inventory.Application/Inventory.Application.csproj Inventory.Application/
COPY Inventory.Infrastructure/Inventory.Infrastructure.csproj Inventory.Infrastructure/
COPY Inventory.Domain/Inventory.Domain.csproj Inventory.Domain/
RUN dotnet restore Inventory.Api/Inventory.Api.csproj
COPY . .
RUN dotnet publish Inventory.Api/Inventory.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://0.0.0.0:5148
EXPOSE 5148
ENTRYPOINT ["dotnet","Inventory.Api.dll"]
