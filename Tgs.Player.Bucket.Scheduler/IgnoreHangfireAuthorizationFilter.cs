using Hangfire.Dashboard;

namespace Tgs.Player.Bucket.Scheduler;

public sealed class IgnoreHangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context) => true;
}