using Hangfire;
using Tgs.Player.Bucket.Scheduler.Jobs;

namespace Tgs.Player.Bucket.Scheduler;

public class PlayerBucketRecurringJob : IPlayerBucketRecurringJob
{
    private readonly IRecurringJobManager _recurringJobManager;

    public PlayerBucketRecurringJob(IRecurringJobManager recurringJobManager)
    {
        _recurringJobManager = recurringJobManager;
    }

    public void RecurringJobs()
    {
        _recurringJobManager.AddOrUpdate<PlayerBucketActivator>("FillInPlayerBucket",
            x => x.FillInAsync(null),
            Cron.Minutely);
    }
}