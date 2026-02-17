# ByteSync.Api - Project Summary

## Overview
ByteSync.Api is a standalone ASP.NET Core Web API that replaces the Azure Functions implementation, enabling deployment on any Windows server without Azure dependencies.

## Quick Stats
- **Controllers**: 10
- **API Endpoints**: ~40
- **Scheduled Jobs**: 3
- **SignalR Hubs**: 1
- **Lines of Code**: ~1,500
- **Dependencies**: MediatR, Quartz.NET, JWT Bearer, SignalR

## Project Structure

```
ByteSync.Api/
├── Configuration/
│   ├── DependencyInjectionConfiguration.cs  # Service registration
│   └── QuartzConfiguration.cs               # Job scheduling
├── Controllers/
│   ├── AuthController.cs                    # Authentication
│   ├── CloudSessionController.cs            # Session management
│   ├── CloudSessionProfileController.cs     # Profile management
│   ├── LobbyController.cs                   # Lobby operations
│   ├── FileTransferController.cs            # File transfers
│   ├── InventoryController.cs               # Inventory management
│   ├── SessionMemberController.cs           # Member management
│   ├── SynchronizationController.cs         # Sync operations
│   ├── TrustController.cs                   # Trust/security
│   └── AnnouncementController.cs            # Public announcements
├── Hubs/
│   └── ByteSyncHub.cs                       # SignalR hub
├── Jobs/
│   ├── RefreshAnnouncementsJob.cs          # Every 2 hours
│   ├── CleanupAzureBlobStorageSnippetsJob.cs # Daily
│   └── CleanupCloudflareR2SnippetsJob.cs   # Daily
├── Helpers/
│   └── ControllerHelper.cs                  # Shared utilities
├── Program.cs                               # Application entry
├── appsettings.json                         # Configuration
└── README.md                                # Documentation
```

## API Endpoint Mapping

### Authentication
- `POST /api/auth/login` - User login
- `POST /api/auth/refreshTokens` - Refresh JWT tokens
- `POST /api/auth/negotiate` - SignalR negotiation

### Cloud Sessions
- `POST /api/session` - Create session
- `POST /api/session/{id}/askPasswordExchangeKey` - Request password key
- `POST /api/session/{id}/validateJoin` - Validate join request
- `POST /api/session/{id}/finalizeJoin` - Finalize join
- `POST /api/session/{id}/askJoin` - Ask to join
- `POST /api/session/{id}/givePasswordExchangeKey` - Give password key
- `POST /api/session/{id}/informPasswordIsWrong` - Report wrong password
- `POST /api/session/{id}/updateSettings` - Update settings
- `POST /api/session/{id}/quit` - Quit session
- `POST /api/session/{id}/reset` - Reset session

### Session Profiles
- `POST /api/cloudSessionProfile` - Create profile
- `POST /api/cloudSessionProfile/{id}/get` - Get profile data
- `POST /api/cloudSessionProfile/{id}/getProfileDetailsPassword` - Get password
- `POST /api/cloudSessionProfile/{id}/delete` - Delete profile

### Lobby
- `POST /api/lobby/join/{profileId}` - Join lobby
- `POST /api/lobby/{id}/sendCloudSessionCredentials` - Send credentials
- `POST /api/lobby/{id}/quit` - Quit lobby
- `POST /api/lobby/{id}/checkInfos` - Send check info
- `POST /api/lobby/{id}/memberStatus` - Update member status

### File Transfer
- `POST /api/session/{id}/file/getUploadUrl` - Get upload URL
- `POST /api/session/{id}/file/getUploadStorageLocation` - Get upload location
- `POST /api/session/{id}/file/getDownloadUrl` - Get download URL
- `POST /api/session/{id}/file/getDownloadStorageLocation` - Get download location
- `POST /api/session/{id}/file/partUploaded` - Assert part uploaded
- `POST /api/session/{id}/file/partDownloaded` - Assert part downloaded
- `POST /api/session/{id}/file/uploadFinished` - Assert upload finished

### Inventory
- `POST /api/session/{id}/inventory/start` - Start inventory
- `POST /api/session/{id}/inventory/{clientId}/dataNode/{nodeId}/dataSource` - Add data source
- `DELETE /api/session/{id}/inventory/{clientId}/dataNode/{nodeId}/dataSource` - Remove data source
- `GET /api/session/{id}/inventory/{clientId}/dataNode/{nodeId}/dataSource` - Get data sources
- `POST /api/session/{id}/inventory/{clientId}/dataNode` - Add data node
- `DELETE /api/session/{id}/inventory/{clientId}/dataNode` - Remove data node
- `GET /api/session/{id}/inventory/{clientId}/dataNode` - Get data nodes

### Session Members
- `GET /api/session/{id}/members/InstanceIds` - Get member IDs
- `GET /api/session/{id}/members` - Get members
- `POST /api/session/{id}/members/{clientId}/generalStatus` - Set status

### Synchronization
- `POST /api/session/{id}/synchronization/start` - Start sync
- `POST /api/session/{id}/synchronization/localCopyIsDone` - Local copy done
- `POST /api/session/{id}/synchronization/dateIsCopied` - Date copied
- `POST /api/session/{id}/synchronization/fileOrDirectoryIsDeleted` - File deleted
- `POST /api/session/{id}/synchronization/directoryIsCreated` - Directory created
- `POST /api/session/{id}/synchronization/memberHasFinished` - Member finished
- `POST /api/session/{id}/synchronization/abort` - Abort sync
- `POST /api/session/{id}/synchronization/errors` - Report errors

### Trust
- `POST /api/trust/startTrustCheck` - Start trust check
- `POST /api/trust/giveMemberPublicKeyCheckData` - Give public key check data
- `POST /api/trust/sendDigitalSignatures` - Send digital signatures
- `POST /api/trust/setAuthChecked` - Set auth checked
- `POST /api/trust/requestTrustPublicKey` - Request trust public key
- `POST /api/trust/informPublicKeyValidationIsFinished` - Inform validation finished
- `POST /api/trust/informProtocolVersionIncompatible` - Inform incompatible version

### Announcements
- `GET /api/announcement` - Get active announcements (public)

## SignalR Hub
- **Endpoint**: `/hubs/bytesync`
- **Authentication**: JWT Bearer token
- **Features**: Real-time push notifications to connected clients

## Scheduled Jobs

### RefreshAnnouncementsJob
- **Schedule**: Every 2 hours (0 0 */2 * * ?)
- **Purpose**: Refresh active announcements from external source
- **Action**: Loads announcements and saves valid ones to database

### CleanupAzureBlobStorageSnippetsJob
- **Schedule**: Daily at midnight (0 0 0 * * ?)
- **Purpose**: Clean up old file snippets from Azure Blob Storage
- **Action**: Sends cleanup request via MediatR

### CleanupCloudflareR2SnippetsJob
- **Schedule**: Daily at midnight (0 0 0 * * ?)
- **Purpose**: Clean up old file snippets from Cloudflare R2
- **Action**: Sends cleanup request via MediatR

## Configuration

### Required Settings
- **AppSettings:Secret** - JWT signing key (REQUIRED)
- **Redis:ConnectionString** - Redis server connection
- **Storage Provider** - Either AzureBlobStorage or CloudflareR2

### Optional Settings
- **AppSettings:JwtDurationInSeconds** - Token expiration (default: 3600)
- **AppSettings:SkipClientsVersionCheck** - Version check (default: false)
- **AppSettings:DefaultStorageProvider** - Storage choice

## Dependencies

### NuGet Packages
- Microsoft.AspNetCore.Authentication.JwtBearer (8.0.23)
- Microsoft.AspNetCore.SignalR (1.1.0)
- MediatR (12.5.0)
- Quartz (3.14.0)
- Quartz.Extensions.Hosting (3.14.0)
- StackExchange.Redis (2.10.1)
- Swashbuckle.AspNetCore (6.6.2)

### Project References
- ByteSync.ServerCommon (shared business logic)

## Build & Run

### Development
```bash
cd src/ByteSync.Api
dotnet run
```

### Production
```bash
dotnet publish -c Release -o ./publish
cd publish
dotnet ByteSync.Api.dll
```

### Docker (Future)
```bash
docker build -t bytesync-api .
docker run -p 5000:5000 bytesync-api
```

## Testing

### Unit Tests
Not yet implemented. Should test:
- Controller endpoints
- Job execution
- Helper methods

### Integration Tests
Not yet implemented. Should test:
- End-to-end API flows
- SignalR connections
- Job scheduling

## Security

### Authentication
- JWT Bearer tokens with HS256 signing
- Claims-based authorization
- Tokens expire after 1 hour (configurable)

### Authorization
- Most endpoints require authentication
- Public endpoints: `/api/announcement`, `/api/auth/login`
- Claims required: ClientId, ClientInstanceId, Version

### Validation
- JWT secret validation on startup
- Configuration validation
- Input validation via model binding

## Performance Considerations

### Caching
- Redis for distributed caching
- Claims caching in HttpContext

### Connection Pooling
- HttpClient factory for HTTP connections
- Redis connection multiplexing
- SignalR connection management

### Async/Await
- All I/O operations are async
- No blocking calls in request pipeline

## Known Limitations

1. **No Azure-specific features**: Azure App Configuration, Azure Key Vault, etc.
2. **Manual scaling**: Unlike Azure Functions, horizontal scaling requires load balancer
3. **Job distribution**: Quartz.NET jobs run on single instance without clustering
4. **SignalR scaling**: No backplane for multi-server scenarios (yet)

## Future Enhancements

- [ ] Docker support with multi-stage builds
- [ ] Health checks and metrics endpoints
- [ ] OpenTelemetry integration
- [ ] Quartz.NET clustering for distributed jobs
- [ ] SignalR backplane (Redis) for scaling
- [ ] Integration tests
- [ ] Unit tests for controllers and jobs
- [ ] API versioning
- [ ] Rate limiting
- [ ] CORS configuration options

## Support & Documentation

- **Project README**: `src/ByteSync.Api/README.md`
- **Migration Guide**: `MIGRATION.md` (root)
- **API Documentation**: Swagger UI at `/swagger` (Development only)

## License
Same as main ByteSync project
