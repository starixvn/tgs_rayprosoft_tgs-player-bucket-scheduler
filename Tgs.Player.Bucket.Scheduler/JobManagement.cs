using System.ComponentModel;
using Hangfire;
using Hangfire.Dashboard.Management.v2.Metadata;
using Hangfire.Dashboard.Management.v2.Support;
using Hangfire.Server;
using Tgs.Fill.In.Player.Bucket.Client;

namespace Tgs.Player.Bucket.Scheduler;

[ManagementPage("PlayerBucket", "填水桶")]
public class JobManagement : IJob
{
    private readonly IFillInPlayerBucketClient _fillInPlayerBucketClient;
    private readonly ILogger<JobManagement> _logger;

    public JobManagement(IFillInPlayerBucketClient fillInPlayerBucketClient, ILogger<JobManagement> logger)
    {
        _fillInPlayerBucketClient = fillInPlayerBucketClient;
        _logger = logger;
    }

    [DisplayName("fill in player bucket")]
    [Description("輸入指定時間填水桶")]
    [AutomaticRetry(Attempts = 1)]
    [DisableConcurrentExecution(90)]
    public async Task FillInPlayerBucketAsync(
        [DisplayData("指定日期時間",
            "2021/02/18 00:00",
            "Default utc offset",
            "2023/09/01 00:00")]
        string dateTime,
        PerformContext context, IJobCancellationToken token)
    {
        _logger.LogInformation($"Start time:{DateTime.UtcNow}");

        if (!DateTimeOffset.TryParse($"{dateTime}:00Z", out var startDate) || startDate >= DateTimeOffset.UtcNow)
        {
            _logger.LogError("DateTime invalided");
        }

        var start = new DateTimeOffset(startDate.Year, startDate.Month, startDate.Day,
            startDate.Hour,
            startDate.Minute, 0, new(0, 0, 0));

        await _fillInPlayerBucketClient.FillPlayerBucketMinutelyAsync(new(start.UtcDateTime));
    }
}