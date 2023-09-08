using Hangfire;
using Hangfire.Server;
using Tgs.Fill.In.Player.Bucket.Client;

namespace Tgs.Player.Bucket.Scheduler.Jobs;

public class PlayerBucketActivator : JobActivator
{
    private readonly IFillInPlayerBucketClient _fillInPlayerBucketClient;
    private readonly ILogger<PlayerBucketActivator> _logger;

    public PlayerBucketActivator(IFillInPlayerBucketClient fillInPlayerBucketClient,
        ILogger<PlayerBucketActivator> logger)
    {
        _fillInPlayerBucketClient = fillInPlayerBucketClient;
        _logger = logger;
    }

    [AutomaticRetry(Attempts = 1)]
    public async Task FillInAsync(PerformContext context)
    {
        var createdTime = context.BackgroundJob.CreatedAt;

        _logger.LogInformation("#### execution time: {createdTime} ####", $"{createdTime:u}");

        var startDate = createdTime.ToUniversalTime().AddMinutes(-3);

        var start = new DateTime(
            startDate.Year,
            startDate.Month,
            startDate.Day,
            startDate.Hour,
            startDate.Minute,
            0,
            DateTimeKind.Utc);

        await _fillInPlayerBucketClient.FillPlayerBucketMinutelyAsync(new(start));
    }
}