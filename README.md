# Запуск проекта

### 1. Подготовка (один раз)
Установить утилиту EF Core:
```bash
dotnet tool install --global dotnet-ef
```

### 2. Запустить БД
```bash
docker compose up -d sqlserver
```

### 3. Восстановить пакеты и применить миграции
```bash
dotnet restore src/Services/Office/Office.slnx
dotnet ef database update --project src/Services/Office/Office.Infrastructure --startup-project src/Services/Office/Office.Api --connection "Server=localhost,1433;Database=OfficeDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;"
```
```bash
dotnet restore src/Services/Services/Services.slnx
dotnet ef database update --project src/Services/Services/Services.Infrastructure --startup-project src/Services/Services/Services.Api --connection "Server=localhost,1433;Database=ServicesDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;"
```

### 4. Запустить API
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
