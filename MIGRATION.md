# Migration Guide: Azure Functions to ASP.NET Core API

This guide helps you migrate from the Azure Functions implementation (`ByteSync.Functions`) to the new standalone ASP.NET Core API (`ByteSync.Api`).

## Overview

The new `ByteSync.Api` project provides the same functionality as `ByteSync.Functions` but runs on any Windows server without Azure dependencies.

## Key Differences

### Architecture
| Aspect | Azure Functions | ASP.NET Core API |
|--------|----------------|------------------|
| Hosting | Azure Functions Runtime | ASP.NET Core Kestrel |
| DI Container | Autofac | ASP.NET Core DI |
| Scheduling | Azure Timer Triggers | Quartz.NET |
| SignalR | Azure SignalR Service | ASP.NET Core SignalR |
| Configuration | Azure App Configuration | appsettings.json + User Secrets |

### Endpoints
All HTTP endpoints are preserved with the `/api` prefix:
- Azure Functions: `/api/auth/login`
- ASP.NET Core API: `/api/auth/login`

**SignalR Hub**:
- Azure Functions: Negotiated via Azure SignalR Service
- ASP.NET Core API: `/hubs/bytesync`

### Scheduled Jobs
| Azure Function | ASP.NET Core Job | Schedule |
|---------------|------------------|----------|
| RefreshAnnouncementsFunction | RefreshAnnouncementsJob | Every 2 hours |
| CleanupBlobFilesFunction | CleanupAzureBlobStorageSnippetsJob | Daily at midnight |
| CleanupCloudflareR2SnippetsFunction | CleanupCloudflareR2SnippetsJob | Daily at midnight |

## Configuration

### 1. appsettings.json

Create or update `appsettings.json`:

```json
{
  "AppSettings": {
    "Secret": "YOUR_SECURE_JWT_SECRET_HERE",
    "JwtDurationInSeconds": 3600,
    "SkipClientsVersionCheck": false,
    "DefaultStorageProvider": "AzureBlobStorage"
  },
  "Redis": {
    "ConnectionString": "localhost:6379",
    "InstanceName": "ByteSync:"
  },
  "AzureBlobStorage": {
    "ConnectionString": "DefaultEndpointsProtocol=https;AccountName=...",
    "ContainerName": "bytesync"
  },
  "CloudflareR2": {
    "AccountId": "your-account-id",
    "AccessKeyId": "your-access-key",
    "SecretAccessKey": "your-secret-key",
    "BucketName": "bytesync"
  }
}
```

### 2. User Secrets (Development)

For development, use User Secrets:

```bash
cd src/ByteSync.Api
dotnet user-secrets set "AppSettings:Secret" "YourDevelopmentSecretKey"
dotnet user-secrets set "Redis:ConnectionString" "localhost:6379"
dotnet user-secrets set "AzureBlobStorage:ConnectionString" "your-connection-string"
```

### 3. Environment Variables (Production)

For production, set environment variables:

```bash
set AppSettings__Secret=YourProductionSecretKey
set Redis__ConnectionString=your-redis-connection
set AzureBlobStorage__ConnectionString=your-storage-connection
```

## Deployment

### Development

```bash
cd src/ByteSync.Api
dotnet run
```

Access:
- HTTP: http://localhost:5000
- HTTPS: https://localhost:5001
- Swagger: https://localhost:5001/swagger

### Production

#### 1. Publish the Application

```bash
dotnet publish src/ByteSync.Api/ByteSync.Api.csproj -c Release -o ./publish
```

#### 2. Copy to Server

Transfer the `publish` folder to your Windows server.

#### 3. Configure Windows Service (Optional)

Create a Windows Service to run the API:

```xml
<!-- bytesync-api.xml -->
<service>
  <id>ByteSyncApi</id>
  <name>ByteSync API</name>
  <description>ByteSync File Synchronization API</description>
  <executable>dotnet</executable>
  <arguments>ByteSync.Api.dll</arguments>
  <workingdirectory>C:\ByteSync\Api</workingdirectory>
</service>
```

Install using [WinSW](https://github.com/winsw/winsw):
```bash
winsw install bytesync-api.xml
winsw start bytesync-api
```

#### 4. Configure IIS (Alternative)

1. Install [.NET 8.0 Hosting Bundle](https://dotnet.microsoft.com/download/dotnet/8.0)
2. Create an Application Pool (No Managed Code)
3. Create a new website pointing to the publish folder
4. Configure bindings (HTTP/HTTPS)

## Client Configuration

### Update SignalR Connection

**Old (Azure Functions)**:
```csharp
var connection = new HubConnectionBuilder()
    .WithUrl("https://your-function-app.azurewebsites.net/api")
    .Build();
```

**New (ASP.NET Core API)**:
```csharp
var connection = new HubConnectionBuilder()
    .WithUrl("https://your-server.com/hubs/bytesync")
    .WithAutomaticReconnect()
    .Build();
```

### API Endpoint Updates

No changes needed if using the same base URL. All endpoints maintain the same routes with the `/api` prefix.

## Testing

### 1. Health Check

```bash
curl https://your-server.com/api/announcement
```

Should return a list of announcements without authentication.

### 2. Authentication

```bash
curl -X POST https://your-server.com/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"test@example.com","password":"password"}'
```

### 3. SignalR Connection

Use the browser console to test:
```javascript
const connection = new signalR.HubConnectionBuilder()
    .withUrl("https://your-server.com/hubs/bytesync")
    .build();

connection.start()
    .then(() => console.log("Connected!"))
    .catch(err => console.error(err));
```

## Monitoring

### Logs

View logs in the console or configure logging in `appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Quartz": "Information"
    }
  }
}
```

### Scheduled Jobs

Check logs for job execution:
- "Refreshing announcements at: {timestamp}"
- "Cleanup Azure BlobStorage Job started at: {timestamp}"
- "Cleanup Cloudflare R2 Job started at: {timestamp}"

## Rollback

If you need to rollback:

1. Keep the Azure Functions deployment running
2. Update client endpoints back to Azure Functions URL
3. No data migration needed as both use the same Redis/Storage backend

## Troubleshooting

### JWT Authentication Fails

- Verify the `Secret` in `AppSettings` matches on all deployments
- Check token expiration (default: 1 hour)
- Ensure HTTPS is configured for production

### SignalR Connection Issues

- Check CORS configuration if clients are on different domains
- Verify WebSocket support on IIS/reverse proxy
- Enable detailed SignalR logging for debugging

### Jobs Not Running

- Check Quartz.NET logs
- Verify Redis connection (used by Quartz for distributed locking)
- Ensure server timezone is UTC or adjust CRON expressions

## Support

For issues or questions:
1. Check the logs first
2. Verify configuration settings
3. Review the README.md in `src/ByteSync.Api/`
4. Open an issue on GitHub
