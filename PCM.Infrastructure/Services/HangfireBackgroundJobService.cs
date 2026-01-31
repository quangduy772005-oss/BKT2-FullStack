using Hangfire;
using PCM.Core.Interfaces;

namespace PCM.Infrastructure.Services;

public class HangfireBackgroundJobService : IBackgroundJobService
{
    public void EnqueueJob<T>(System.Linq.Expressions.Expression<Func<T, Task>> methodCall)
    {
        BackgroundJob.Enqueue(methodCall);
    }

    public void ScheduleJob<T>(System.Linq.Expressions.Expression<Func<T, Task>> methodCall, TimeSpan delay)
    {
        BackgroundJob.Schedule(methodCall, delay);
    }
}
