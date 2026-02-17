# ByteSync.Api

This is an ASP.NET Core Web API project that provides the server-side functionality for ByteSync. It replaces the Azure Functions implementation with a standalone API that can be deployed on any Windows server without Azure dependencies.

## Features

- **RESTful API**: All HTTP endpoints migrated from Azure Functions to ASP.NET Core Controllers
- **SignalR Hub**: Real-time communication for client-server updates
- **Scheduled Jobs**: Background tasks using Quartz.NET for maintenance operations
- **JWT Authentication**: Built-in JWT token authentication and authorization
- **Dependency Injection**: Uses ASP.NET Core's native DI container
- **No Azure Dependencies**: Runs on standalone Windows servers

## Architecture

### Controllers
- **AuthController**: Authentication (login, token refresh, SignalR negotiation)
- **CloudSessionController**: Session management (create, join, update, quit, reset)
- **CloudSessionProfileController**: Profile management
- **LobbyController**: Lobby operations for multi-client sessions
- **FileTransferController**: File upload/download operations
- **InventoryController**: Data source and node inventory management
- **SessionMemberController**: Member management within sessions
- **SynchronizationController**: Synchronization operations
- **TrustController**: Trust and security operations
- **AnnouncementController**: Public announcements

### SignalR Hub
- **ByteSyncHub**: Real-time push notifications to connected clients

### Scheduled Jobs (Quartz.NET)
- **RefreshAnnouncementsJob**: Runs every 2 hours to refresh active announcements
- **CleanupAzureBlobStorageSnippetsJob**: Daily cleanup of old blob storage files
- **CleanupCloudflareR2SnippetsJob**: Daily cleanup of old Cloudflare R2 files

## Configuration

Edit `appsettings.json` or use User Secrets for sensitive configuration:

```json
{
  "AppSettings": {
    "Secret": "YourSecretKeyHere",
    "JwtDurationInSeconds": 3600,
    "SkipClientsVersionCheck": false,
    "DefaultStorageProvider": "AzureBlobStorage"
  },
  "Redis": {
    "ConnectionString": "localhost:6379",
    "InstanceName": "ByteSync:"
  },
  "AzureBlobStorage": {
    "ConnectionString": "your-connection-string",
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

## Running the Application

### Development
```bash
dotnet run --project src/ByteSync.Api/ByteSync.Api.csproj
```

The API will be available at:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`
- Swagger UI: `https://localhost:5001/swagger`

### Production
```bash
dotnet publish src/ByteSync.Api/ByteSync.Api.csproj -c Release -o ./publish
cd publish
dotnet ByteSync.Api.dll
```

## API Endpoints

All endpoints follow the pattern `/api/{controller}/{action}`, for example:
- `POST /api/auth/login`
- `POST /api/session` (create session)
- `POST /api/session/{sessionId}/quit`
- `GET /api/announcement`

See Swagger UI for complete API documentation when running in Development mode.

## SignalR Hub

Clients connect to the SignalR hub at `/hubs/bytesync` for real-time updates.

## Requirements

- .NET 8.0 SDK or later
- Redis server (for caching and distributed operations)
- Azure Blob Storage or Cloudflare R2 (for file storage)

## Migration Notes

This project replaces the `ByteSync.Functions` Azure Functions implementation with a standalone ASP.NET Core API. The main differences:

1. **No Azure-specific dependencies**: Removed Azure Functions Worker packages
2. **Standard Controllers**: Azure Functions become standard MVC controllers
3. **Quartz.NET**: Replaces Azure Timer Triggers for scheduled jobs
4. **ASP.NET Core SignalR**: Standard SignalR hub instead of Azure SignalR Service
5. **Built-in DI**: Uses ASP.NET Core DI instead of Autofac

## Development

The project uses:
- ASP.NET Core 8.0
- MediatR for CQRS pattern
- Quartz.NET for job scheduling
- JWT Bearer authentication
- SignalR for real-time communication
