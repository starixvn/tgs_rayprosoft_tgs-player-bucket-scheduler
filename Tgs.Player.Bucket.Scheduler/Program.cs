using Hangfire;
using Hangfire.Console;
using Hangfire.Dashboard.Management.v2;
using Hangfire.Dashboard.Management.v2.Support;
using Hangfire.Redis.StackExchange;
using Tgs.Fill.In.Player.Bucket.Client.DependencyInjection;
using Tgs.Player.Bucket.Scheduler;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseSentry();
builder.Configuration
    .AddJsonFile("/config/appsettings.json", true)
    .AddEnvironmentVariables();

builder.Services
    .AddFillInPlayerBucketClient((options, sp) =>
    {
        var config = sp.GetRequiredService<IConfiguration>();
        options.Url = config.GetValue<Uri>("PlayerBucketScheduler:FillInPlayerBucket")!;
    });


builder.Services.AddHangfire((sp, config) =>
{
    var connectionString = builder.Configuration.GetConnectionString("Redis")!;

    var options = new RedisStorageOptions
    {
        Prefix = builder.Configuration.GetValue<string>("PlayerBucketScheduler:Prefix")
    };
    _ = config
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseRedisStorage(connectionString, options)
        .UseConsole();

    config.UseManagementPages(new[] { typeof(JobManagement).Assembly });
});

builder.Services.AddHangfireServer(options =>
{
    options.Queues = JobsHelper.GetAllQueues().ToArray();
});

builder.Services.AddHealthChecks();

builder.Services.AddSingleton<IPlayerBucketRecurringJob, PlayerBucketRecurringJob>();

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseHangfireDashboard("/hangfire",
    new()
    {
        Authorization = new[]
        {
            new IgnoreHangfireAuthorizationFilter()
        }
    });

app.JobRegistration();

app.MapHealthChecks("/healthz");

app.Run();