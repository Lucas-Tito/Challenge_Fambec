.PHONY: build up down logs clean health

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