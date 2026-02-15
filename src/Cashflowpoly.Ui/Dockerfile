# build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Cashflowpoly.sln ./
COPY src/Cashflowpoly.Ui/Cashflowpoly.Ui.csproj src/Cashflowpoly.Ui/
RUN dotnet restore src/Cashflowpoly.Ui/Cashflowpoly.Ui.csproj

COPY . .
RUN dotnet publish src/Cashflowpoly.Ui/Cashflowpoly.Ui.csproj -c Release -o /app/publish

# runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Cashflowpoly.Ui.dll"]
