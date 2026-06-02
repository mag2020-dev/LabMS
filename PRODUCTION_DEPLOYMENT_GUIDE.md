# Production Deployment Guide

## 1. Environment Configuration

### CORS Configuration
In production, you must explicitly allow your frontend domain. Update `appsettings.Production.json`:

```json
"Cors": {
  "AllowedOrigins": [
    "https://your-production-domain.com",
    "https://admin.your-production-domain.com"
  ]
}
```

### Security Headers (HSTS)
HSTS is automatically enabled in production. Ensure your load balancer or reverse proxy forwards the `X-Forwarded-Proto` header so the application knows it's running over HTTPS.

### JWT Secret
Set the `JWT_SECRET_KEY` environment variable to a strong, random string (min 32 chars).

## 2. Database
Run migrations against your production database:
```bash
dotnet ef database update --environment Production
```

## 3. Logging
Configure a centralized logging sink in `appsettings.Production.json` or use the existing file logging which writes to `logs/`.
