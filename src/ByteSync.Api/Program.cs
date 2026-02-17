using System.Reflection;
using System.Text.Json;
using ByteSync.Api.Configuration;
using ByteSync.Api.Hubs;
using ByteSync.Common.Controls.Json;
using ByteSync.ServerCommon.Business.Settings;
using ByteSync.ServerCommon.Commands.Inventories;
using ByteSync.ServerCommon.Helpers;
using Quartz;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>(optional: true, reloadOnChange: false);
builder.Configuration.AddEnvironmentVariables();

Console.WriteLine($"Current Environment: {builder.Environment.EnvironmentName}");

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        JsonSerializerOptionsHelper.SetOptions(options.JsonSerializerOptions);
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient();

var appSettingsSection = builder.Configuration.GetSection("AppSettings");
builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection("Redis"));
builder.Services.Configure<AzureBlobStorageSettings>(builder.Configuration.GetSection("AzureBlobStorage"));
builder.Services.Configure<CloudflareR2Settings>(builder.Configuration.GetSection("CloudflareR2"));
builder.Services.Configure<AppSettings>(appSettingsSection);
var appSettings = appSettingsSection.Get<AppSettings>();

if (appSettings != null)
{
    if (string.IsNullOrWhiteSpace(appSettings.Secret) || 
        appSettings.Secret.Contains("CHANGE_THIS", StringComparison.OrdinalIgnoreCase) ||
        appSettings.Secret == "YourSecretKeyHere")
    {
        throw new InvalidOperationException(
            "JWT Secret must be configured in AppSettings. " +
            "Please set a secure secret key in appsettings.json or User Secrets.");
    }
    
    builder.Services.AddClaimAuthorization();
    builder.Services.AddJwtAuthentication(appSettings.Secret);
}

builder.Services.AddSignalR()
    .AddJsonProtocol(options =>
    {
        JsonSerializerOptionsHelper.SetOptions(options.PayloadSerializerOptions);
    });

builder.Services.AddMediatR(cfg => 
    cfg.RegisterServicesFromAssemblies(
        Assembly.GetExecutingAssembly(), 
        typeof(AddDataSourceRequest).Assembly));

DependencyInjectionConfiguration.RegisterServices(builder.Services, builder.Configuration);

QuartzConfiguration.ConfigureJobs(builder.Services);

builder.Services.AddQuartzHostedService(options =>
{
    options.WaitForJobsToComplete = true;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ByteSyncHub>("/hubs/bytesync");

app.Run();
