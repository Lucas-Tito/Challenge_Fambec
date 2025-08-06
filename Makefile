.PHONY: build up down logs clean health dev-server dev-client dev-db dev-stop

# Development commands (without Docker)
dev-db:
	@echo "Starting SQL Server in Docker for development..."
	docker-compose up -d sqlserver-only

dev-server:
	@echo "Starting API server on host..."
	cd Challenge_Fambec.Server && dotnet watch run --launch-profile http

dev-client:
	@echo "Starting Blazor client on host..."
	cd Challenge_Fambec.Client && dotnet watch run --launch-profile http

dev-stop:
	@echo "Stopping development SQL Server..."
	docker-compose down

# Full development setup
dev: dev-db
	@echo "Starting development environment..."
	@echo "Run 'make dev-server' in one terminal and 'make dev-client' in another"

# Build all services
build:
	docker-compose build

# Start services
up:
	docker-compose up -d

# Stop services
down:
	docker-compose down

# View logs
logs:
	docker-compose logs -f

# View logs for specific service
logs-api:
	docker-compose logs -f api

logs-client:
	docker-compose logs -f client

logs-db:
	docker-compose logs -f sqlserver

# Check health status
health:
	@echo "Checking service health..."
	@docker-compose ps

# Clean up everything
clean:
	docker-compose down -v
	docker system prune -f

# Restart specific service
restart-api:
	docker-compose restart api

restart-client:
	docker-compose restart client