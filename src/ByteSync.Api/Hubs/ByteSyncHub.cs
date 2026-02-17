using ByteSync.Common.Interfaces.Hub;
using ByteSync.ServerCommon.Interfaces.Services.Clients;
using Microsoft.AspNetCore.SignalR;

namespace ByteSync.Api.Hubs;

public class ByteSyncHub : Hub<IHubByteSyncPush>
{
    private readonly IClientsService _clientsService;
    private readonly ILogger<ByteSyncHub> _logger;

    public ByteSyncHub(IClientsService clientsService, ILogger<ByteSyncHub> logger)
    {
        _clientsService = clientsService;
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        try
        {
            var userId = Context.UserIdentifier;
            var connectionId = Context.ConnectionId;
            
            if (!string.IsNullOrEmpty(userId))
            {
                await _clientsService.OnClientConnected(userId, connectionId);
                _logger.LogInformation("{ConnectionId} has connected", connectionId);
            }
            
            await base.OnConnectedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during OnConnectedAsync");
        }
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.UserIdentifier;
        var connectionId = Context.ConnectionId;
        
        _logger.LogInformation("{ConnectionId} has disconnected, UserId:{UserId}, Reason:{Reason}", 
            connectionId, userId, exception?.Message);
        
        return base.OnDisconnectedAsync(exception);
    }
}
