namespace Tribitgroup.Framework.Shared.Interfaces
{
    public interface IScheduler
    {
        Task ScheduleAsync<T>(TimeSpan time, Func<IServiceProvider, T?, Task> action, T? input);
    }
}