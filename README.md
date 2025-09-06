# Faces Hunter – Backend (.NET 9 Web API)

Backend service for the **Faces Hunter** project. Provides RESTful APIs for managing missing person reports, authentication/authorization with JWT, secure image upload, AI matching (FaceNet/ArcFace via FastAPI), and notifications.

---

## 🚀 Key Features (Backend Only)
- **JWT Authentication & Authorization** (Users, Admins).
- **Missing Persons CRUD**: create/update status (Missing/Found), details.
- **Secure Image Upload**: validation, size/type limits, unique naming, safe storage.
- **Search by Image**: send image to AI service (FastAPI) → store embeddings → similarity search.
- **Notifications**: per-user notifications + admin alerts on new reports.
- **EF Core + SQL Server**: migrations, optimized queries, pagination & filtering.
- **Serilog + Application Insights** logging & monitoring.
- **CORS**: restricted origins for frontend only.
- **Swagger/OpenAPI**: auto API docs.

---

## 🧱 Architecture
- **ASP.NET Core 9 Web API**
- **Entity Framework Core** (Code‑First, SQL Server)
- **Identity / JWT** for auth
- **Services layer** for business logic
- **Repositories (optional)** for data access abstraction
- **Integration layer** for AI (FastAPI endpoints)

```
Backend/
├── Controllers/
├── Services/
├── Interfaces/
├── Models/
├── DTOs/
├── Data/ (DbContext, Seed)
├── Migrations/
├── Middleware/
└── Helpers/ (Mapping, Extensions, Constants)
```

---

## 🧰 Tech Stack
- .NET 9 Web API, EF Core, ASP.NET Core Identity
- SQL Server
- Swagger (Swashbuckle)
- Integration with **FastAPI** (FaceNet/ArcFace) via HTTP

---

## 📋 Prerequisites
- .NET SDK 9.0
- SQL Server (Express or higher)
- Visual Studio 2022 or `dotnet` CLI
- AI Service URL(s) (FastAPI) running locally or remote
- SMTP credentials (for forgot password / notifications)

---

## ⚙️ Configuration

Create/update `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=FacesHunterDb;Trusted_Connection=True;TrustServerCertificate=True"
  },
  "Jwt": {
    "Issuer": "FacesHunter",
    "Audience": "FacesHunterClient",
    "Key": "REPLACE_WITH_LONG_RANDOM_SECRET_KEY",
    "AccessTokenMinutes": 60
  },
  "Cors": {
    "AllowedOrigins": [ "https://your-frontend-domain.com", "http://localhost:3000" ]
  },
  "EmailSettings": {
    "MailServer": "smtp.gmail.com",
    "FromEmail": "your-email@gmail.com",
    "Password": "your-app-password",
    "MailPort": 587
  },
  "AI": {
    "BaseUrl": "http://localhost:8000",
    "Endpoints": {
      "ExtractEmbedding": "/api/face/embedding",
      "Search": "/api/face/search"
    },
    "SimilarityThreshold": 0.75
  },
  "Serilog": {
    "Using": [ "Serilog.Sinks.Console", "Serilog.Sinks.File" ],
    "MinimumLevel": "Information",
    "WriteTo": [
      { "Name": "Console" },
      { "Name": "File", "Args": { "path": "Logs/log-.txt", "rollingInterval": "Day" } }
    ]
  },
  "AllowedHosts": "*"
}
```

### Program.cs highlights
- Configure **DbContext**, **Identity**, **JWT**, **CORS**, **Swagger**, **Serilog**.
- Add **IImageService**, **IAIService**, **INotificationService** to DI.

---

## 🗄️ Database & Migrations

```bash
# from Backend project folder
dotnet ef migrations add InitialCreate
dotnet ef database update
```

If EF tools not installed:
```bash
dotnet tool install --global dotnet-ef
```

---

## ▶️ Run

```bash
dotnet run
# Swagger at:
# https://localhost:5001/swagger  or  http://localhost:5000/swagger
```

**Remember:** Start your **FastAPI AI service** too (for embeddings & search).

---

## 🔌 API Endpoints (Summary)

### Auth
- `POST /api/auth/register`
- `POST /api/auth/login`
- `POST /api/auth/refresh` (optional)
- `POST /api/auth/forgot-password`
- `POST /api/auth/reset-password`

### Users (Admin)
- `GET /api/users` (paging/filter)
- `GET /api/users/{id}`
- `POST /api/users` (create by admin)
- `PUT /api/users/{id}`
- `DELETE /api/users/{id}`
- `PUT /api/users/{id}/role` (assign/remove roles)

### Missing Persons
- `POST /api/missing` (create report + images)
- `GET /api/missing` (list with filters: status, date, city, keyword, paging)
- `GET /api/missing/{id}`
- `PUT /api/missing/{id}` (update details)
- `PATCH /api/missing/{id}/status` (Missing/Found/Closed)
- `DELETE /api/missing/{id}` (owner/admin)

### Images
- `POST /api/images/upload` (IFormFile image)
- `GET /api/images/{fileName}` (serve if public) or signed URLs

### AI
- `POST /api/ai/extract-embedding` (internal use)
- `POST /api/ai/search` (upload image → similar cases)

### Notifications
- `GET /api/notifications` (current user)
- `POST /api/notifications` (admin/system)
- `PATCH /api/notifications/{id}/read`

---

## 🧪 Sample Requests

### Register
```bash
curl -X POST https://localhost:5001/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{ "fullName":"Sara Samir", "email":"sara@example.com", "password":"P@ssw0rd!" }'
```

### Login
```bash
curl -X POST https://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{ "email":"sara@example.com", "password":"P@ssw0rd!" }'
```

### Create Missing Report (multipart with image)
```bash
curl -X POST https://localhost:5001/api/missing \
  -H "Authorization: Bearer <JWT>" \
  -F "FullName=Person X" \
  -F "City=Cairo" \
  -F "Age=25" \
  -F "Notes=Last seen in Nasr City" \
  -F "Image=@/path/to/photo.jpg"
```

### Search by Image
```bash
curl -X POST https://localhost:5001/api/ai/search \
  -H "Authorization: Bearer <JWT>" \
  -F "Image=@/path/to/photo.jpg"
```

---

## 🔐 Security Practices
- Strong password policy & email verification (optional).
- Validate images: mime type + max size (e.g., 5 MB) + safe file names.
- Store images outside web root or use object storage.
- Use HTTPS, secure cookies, short‑lived JWTs + refresh tokens (optional).
- Limit data exposure with DTOs + AutoMapper.
- Rate limiting (e.g., AspNetCoreRateLimit) for sensitive endpoints.
- Log security events with Serilog + Application Insights.

---

## 🛠 Troubleshooting
- **warn**: “Possible null reference for IFormFile” → validate `imageFile != null` before use.
- **CORS blocked**: ensure frontend origin is added under `Cors:AllowedOrigins`.
- **500 on AI search**: check AI service URL, network, and payload format.
- **EF migration errors**: clear and re‑create (`dotnet ef migrations remove` then add).

---

## 🤝 Contributing
- Use feature branches and PRs.
- Follow REST & DTO conventions.
- Add unit/integration tests for services and controllers where possible.

---

## 📄 License
Internal/Academic use unless license file is added.
