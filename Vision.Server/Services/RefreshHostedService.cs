using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Vision.Core;

namespace Vision.Server.Services
{
    internal class RefreshHostedService : IHostedService
    {
        private readonly ILogger logger;

        public RefreshHostedService(IServiceProvider services, ILogger<RefreshHostedService> logger)
        {
            Services = services;
            this.logger = logger;
        }

        public IServiceProvider Services { get; }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Hosted Service is starting.");

            await DoWorkAsync(cancellationToken);
        }

        private async Task DoWorkAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Hosted Service is working.");

            while(!cancellationToken.IsCancellationRequested)
            {
                logger.LogInformation("Hosted Service is checking for items...");

                using (IServiceScope scope = Services.CreateScope())
                {
                    IRefreshService refreshService = scope.ServiceProvider.GetRequiredService<IRefreshService>();
                    await refreshService.NextRefreshTaskAsync();
                }

                await Task.Delay(TimeSpan.FromSeconds(30));
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Consume Scoped Service Hosted Service is stopping.");

            return Task.CompletedTask;
        }
    }

    public interface IBackgroundTaskQueue
    {
        void QueueBackgroundWorkItem(Func<CancellationToken, Task> workItem);

        Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken);
    }

    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private readonly ConcurrentQueue<Func<CancellationToken, Task>> workItems = new ConcurrentQueue<Func<CancellationToken, Task>>();
        private readonly SemaphoreSlim signal = new SemaphoreSlim(0);

        public void QueueBackgroundWorkItem(Func<CancellationToken, Task> workItem)
        {
            if (workItem == null)
                throw new ArgumentNullException(nameof(workItem));

            workItems.Enqueue(workItem);
            signal.Release();
        }

        public async Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken)
        {
            await signal.WaitAsync(cancellationToken);
            workItems.TryDequeue(out var workItem);
            return workItem;
        }
    }
}
