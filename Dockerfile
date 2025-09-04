# Multi-stage Dockerfile for SubscriptionTracker API (builds from .NET 9 SDK and runs on ASP.NET runtime)

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy everything and restore/publish the API project
COPY . .
RUN dotnet restore "src/Api/SubscriptionTracker.Api.csproj"
RUN dotnet publish "src/Api/SubscriptionTracker.Api.csproj" -c Release -o /app/publish --no-restore

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

# Copy publish output
COPY --from=build /app/publish ./

ENTRYPOINT ["dotnet", "SubscriptionTracker.Api.dll"]
