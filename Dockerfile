# ---------- BUILD STAGE ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file
COPY uc15.sln .

# Copy all project folders
COPY QuantityMeasurementApp.API/ QuantityMeasurementApp.API/
COPY QuantityMeasurementApp.Business/ QuantityMeasurementApp.Business/
COPY QuantityMeasurementApp.Controller/ QuantityMeasurementApp.Controller/
COPY QuantityMeasurementApp.Entity/ QuantityMeasurementApp.Entity/
COPY QuantityMeasurementApp.Repository/ QuantityMeasurementApp.Repository/

# Restore dependencies
RUN dotnet restore uc15.sln

# Publish API
RUN dotnet publish QuantityMeasurementApp.API/QuantityMeasurementApp.API.csproj -c Release -o /app/publish

# ---------- RUNTIME STAGE ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "QuantityMeasurementApp.API.dll"]