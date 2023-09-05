using Hangfire;
using Hangfire.Server;
using Hangfire.Storage;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.Fill.In.Player.Bucket.Client;
using Tgs.Fill.In.Player.Bucket.Client.Models;
using Tgs.Player.Bucket.Scheduler.Jobs;

namespace Tgs.Player.Bucket.Scheduler.UnitTest
{
    public class PlayerBucketTests
    {
        [Test]
        public async Task Fill_In_Player_Bucket()
        {
            var fakeClient = Substitute.For<IFillInPlayerBucketClient>();

            var sut = new PlayerBucketActivator(
                fakeClient,
                NullLogger<PlayerBucketActivator>.Instance);

            var createdOn = DateTime.Parse("2023/09/01 01:55:00Z").ToUniversalTime();   

            var request = PerformContextMock(createdOn);

            await sut.FillInAsync(request.Value);

            await fakeClient.Received(1)
                .FillPlayerBucketMinutelyAsync(
                    Arg.Is<FillPlayerBucketMinutelyModel>(x => createdOn.AddMinutes(-3) == x.StartDate));
        }

        private static Lazy<PerformContext> PerformContextMock(DateTime createdOn)
        {
            var storage = Substitute.For<JobStorage>();
            var connection = Substitute.For<IStorageConnection>();
            var backgroundJob = Substitute.For<BackgroundJobMock>();
            var cancellationToken = Substitute.For<IJobCancellationToken>();

            backgroundJob.CreatedAt = createdOn;

            return new Lazy<PerformContext>(
                () => new PerformContext(storage, connection, backgroundJob.Object, cancellationToken));
        }
    }
}