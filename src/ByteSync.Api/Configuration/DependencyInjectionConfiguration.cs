using ByteSync.Common.Interfaces.Hub;
using ByteSync.ServerCommon.Interfaces.Repositories;
using Microsoft.AspNetCore.SignalR;

namespace ByteSync.Api.Configuration;

public static class DependencyInjectionConfiguration
{
    public static void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        var serverCommonAssembly = typeof(IClientsRepository).Assembly;
        
        var repositoryTypes = serverCommonAssembly.GetTypes()
            .Where(t => t.Name.EndsWith("Repository") && !t.IsInterface && !t.IsAbstract);
        
        foreach (var type in repositoryTypes)
        {
            var interfaces = type.GetInterfaces();
            foreach (var iface in interfaces)
            {
                services.AddScoped(iface, type);
            }
        }
        
        var genericRepositoryTypes = serverCommonAssembly.GetTypes()
            .Where(t => t.Name.Contains("Repository`") && t.IsGenericTypeDefinition && !t.IsInterface);
            
        foreach (var genericType in genericRepositoryTypes)
        {
            var interfaces = genericType.GetInterfaces();
            foreach (var iface in interfaces.Where(i => i.IsGenericType))
            {
                services.AddScoped(iface.GetGenericTypeDefinition(), genericType);
            }
        }
        
        var serviceTypes = serverCommonAssembly.GetTypes()
            .Where(t => t.Name.EndsWith("Service") && !t.IsInterface && !t.IsAbstract);
        
        foreach (var type in serviceTypes)
        {
            var interfaces = type.GetInterfaces();
            foreach (var iface in interfaces)
            {
                services.AddScoped(iface, type);
            }
        }
        
        var factoryTypes = serverCommonAssembly.GetTypes()
            .Where(t => t.Name.EndsWith("Factory") && !t.IsInterface && !t.IsAbstract);
        
        foreach (var type in factoryTypes)
        {
            var interfaces = type.GetInterfaces();
            foreach (var iface in interfaces)
            {
                services.AddScoped(iface, type);
            }
        }
        
        var loaderTypes = serverCommonAssembly.GetTypes()
            .Where(t => t.Name.EndsWith("Loader") && !t.IsInterface && !t.IsAbstract);
        
        foreach (var type in loaderTypes)
        {
            var interfaces = type.GetInterfaces();
            foreach (var iface in interfaces)
            {
                services.AddScoped(iface, type);
            }
        }
        
        var mapperTypes = serverCommonAssembly.GetTypes()
            .Where(t => t.Name.EndsWith("Mapper") && !t.IsInterface && !t.IsAbstract);
        
        foreach (var type in mapperTypes)
        {
            var interfaces = type.GetInterfaces();
            foreach (var iface in interfaces)
            {
                services.AddScoped(iface, type);
            }
        }
    }
}
