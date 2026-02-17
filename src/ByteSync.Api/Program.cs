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

builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();
});

builder.Services.AddQuartzHostedService(options =>
{
    options.WaitForJobsToComplete = true;
});

QuartzConfiguration.ConfigureJobs(builder.Services);

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
