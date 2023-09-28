namespace Tribitgroup.Framewok.Shared.Interfaces
{
    public class TaskDelayScheduler : IScheduler
    {
        protected IServiceProvider ServiceProvider { get; init; }

        public TaskDelayScheduler(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public async Task ScheduleAsync<T>(TimeSpan time, Func<IServiceProvider ,T?, Task> action, T? input)
        {
            await Task.Delay(time.Microseconds);
            await action(ServiceProvider, input);
        }
    }
}