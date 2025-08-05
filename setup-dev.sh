#!/bin/bash

echo "Setting up development environment..."

# Create HTTPS certificate for development
echo "Creating HTTPS certificate..."
dotnet dev-certs https -ep ~/.aspnet/https/aspnetapp.pfx -p password
dotnet dev-certs https --trust

# Build and start containers
echo "Building and starting containers..."
docker-compose up --build -d

echo "Waiting for services to be healthy..."
sleep 30

# Check health status
echo "Checking service health..."
docker-compose ps

echo ""
echo "🚀 Development environment is ready!"
echo ""
echo "Services available at:"
echo "- Blazor Client: http://localhost:5000"
echo "- Web API: https://localhost:7001"
echo "- Swagger UI: https://localhost:7001/swagger"
echo "- Health Check: https://localhost:7001/health"
echo "- SQL Server: localhost:1433 (sa/YourStrong@Passw0rd)"
echo ""
echo "To stop: docker-compose down"
echo "To view logs: docker-compose logs -f [service-name]"