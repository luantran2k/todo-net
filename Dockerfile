# Use official .NET SDK image for build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Use runtime-only image for final container
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final

WORKDIR /app
COPY --from=build /app/out .

ENV ASPNETCORE_ENVIRONMENT=Production
# Expose ports (HTTP and HTTPS)
EXPOSE 5000
EXPOSE 5001

# Start the app
ENTRYPOINT ["dotnet", "MyApiProject.dll"]
