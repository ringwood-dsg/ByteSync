using System.Security.Claims;
using ByteSync.ServerCommon.Business.Auth;
using ByteSync.ServerCommon.Interfaces.Repositories;

namespace ByteSync.Api.Helpers;

public static class ControllerHelper
{
    public static async Task<Client> GetClientFromContext(HttpContext context, IClientsRepository clientsRepository)
    {
        var clientInstanceId = context.User.Claims
            .FirstOrDefault(c => c.Type.Equals(AuthConstants.CLAIM_CLIENT_INSTANCE_ID))?.Value;
        
        if (string.IsNullOrEmpty(clientInstanceId))
        {
            throw new InvalidOperationException("Client instance ID not found in claims");
        }
        
        var client = await clientsRepository.Get(clientInstanceId);
        if (client == null)
        {
            throw new InvalidOperationException($"Client not found: {clientInstanceId}");
        }
        
        return client;
    }
}
