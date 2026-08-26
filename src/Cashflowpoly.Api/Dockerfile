# Fungsi file: Membangun dan menjalankan image production API .NET secara multi-stage.
# build
FROM mcr.microsoft.com/dotnet/sdk:10.0.200 AS build
WORKDIR /src

COPY Cashflowpoly.sln ./
COPY src/Cashflowpoly.Api/Cashflowpoly.Api.csproj src/Cashflowpoly.Api/
RUN dotnet restore src/Cashflowpoly.Api/Cashflowpoly.Api.csproj

COPY . .
RUN dotnet publish src/Cashflowpoly.Api/Cashflowpoly.Api.csproj -c Release -o /app/publish

# runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0.4 AS runtime
WORKDIR /app
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/* \
    && mkdir -p /home/app/.aspnet/DataProtection-Keys \
    && chown -R app:app /home/app/.aspnet
COPY --from=build --chown=app:app /app/publish .
USER app
EXPOSE 8080
ENTRYPOINT ["dotnet", "Cashflowpoly.Api.dll"]
