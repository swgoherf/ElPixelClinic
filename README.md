# Запуск проекта

### 1. Запустить БД
```bash
docker compose up -d sqlserver
```

### 2. Накатить миграции
```bash
dotnet ef database update --project src/Services/Office/Office.Infrastructure --startup-project src/Services/Office/Office.Api --connection "Server=localhost,1433;Database=OfficeDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;"
```
```bash
dotnet ef database update --project src/Services/Services/Services.Infrastructure --startup-project src/Services/Services/Services.Api --connection "Server=localhost,1433;Database=ServicesDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;"
```

### 3. Запустить API
```bash
docker compose up -d --build
```

---

### Ссылки:
* **Office API:** http://localhost:5253/swagger
* **Services API:** http://localhost:5109/swagger

### Остановить:
```bash
docker compose down
```
