# Dev Quickstart

## Docker Hard Reset

```bash
sudo docker-compose down -v
sudo docker-compose build --no-cache
sudo docker-compose up
````

Expected Ports:

* Client in Docker: [http://0.0.0.0:80](http://0.0.0.0:80)
* Client in Host: [http://0.0.0.0:80](http://0.0.0.0:80)
* Server in Docker: [http://localhost:5001](http://localhost:5001)
* Server in Host is dynamic, so checkout terminal logs.

---

## 📦 Migrations

```bash
dotnet ef migrations add <migration_name>
```

---

## 🖥️ Run Locally (Host)

```bash
# In project root
dotnet restore
dotnet build

# Start database only
docker-compose --profile db-only up sqlserver-only

# Run backend and frontend
dotnet watch --project Challenge_Fambec.Server     # Port 5001 or next available
dotnet watch --project Challenge_Fambec.Client     # Port 5000

# Stop project containers
sudo docker stop $(sudo docker ps -aq --filter "name=challenge-fambec")
```

---

## 🔎 List Mock Partners

```bash
curl http://localhost:5214/api/products/1
```

## SQLCMD

```bash
sqlcmd -S localhost -U SA -P "YourStrong@Passw0rd"
```

```sql
USE Challenge_FambecDb
GO

SELECT TABLE_SCHEMA, TABLE_NAME
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE = 'BASE TABLE';
GO

SELECT * FROM Products;
GO
```

