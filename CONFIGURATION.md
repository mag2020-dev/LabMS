# LabMS Configuration Guide

## Required Environment Variables

### JWT Configuration
- `JWT_SECRET_KEY`: A secure secret key for JWT token signing (minimum 32 characters)
  - Example: `JWT_SECRET_KEY=your-super-secure-jwt-secret-key-here-minimum-32-characters`

### Database Configuration
- `ConnectionStrings__DefaultConnection`: SQL Server connection string
  - Example: `ConnectionStrings__DefaultConnection=Server=localhost;Database=LabMS;Trusted_Connection=True;TrustServerCertificate=True`

### Application Configuration
- `ASPNETCORE_ENVIRONMENT`: Environment setting (Development, Staging, Production)

## Setting Environment Variables

### Windows (PowerShell)
```powershell
$env:JWT_SECRET_KEY="your-super-secure-jwt-secret-key-here-minimum-32-characters"
$env:ConnectionStrings__DefaultConnection="Server=localhost;Database=LabMS;Trusted_Connection=True;TrustServerCertificate=True"
$env:ASPNETCORE_ENVIRONMENT="Development"
```

### Windows (Command Prompt)
```cmd
set JWT_SECRET_KEY=your-super-secure-jwt-secret-key-here-minimum-32-characters
set ConnectionStrings__DefaultConnection=Server=localhost;Database=LabMS;Trusted_Connection=True;TrustServerCertificate=True
set ASPNETCORE_ENVIRONMENT=Development
```

### Linux/macOS
```bash
export JWT_SECRET_KEY="your-super-secure-jwt-secret-key-here-minimum-32-characters"
export ConnectionStrings__DefaultConnection="Server=localhost;Database=LabMS;Trusted_Connection=True;TrustServerCertificate=True"
export ASPNETCORE_ENVIRONMENT="Development"
```

## Security Notes

- **NEVER** commit the actual JWT secret to version control
- Use a strong, randomly generated secret key (minimum 32 characters)
- In production, use Azure Key Vault or similar secure configuration management
- Rotate JWT secrets regularly
