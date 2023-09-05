namespace Tgs.Player.Bucket.Scheduler;

public static class Extension
{
    public static IApplicationBuilder JobRegistration(this IApplicationBuilder app)
    {
        var applicationServices = app.ApplicationServices;

        var recurringJob = applicationServices.GetRequiredService<IPlayerBucketRecurringJob>();

        recurringJob.RecurringJobs();

        return app;
    }
}