namespace Vision.Web.Core
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Storage;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    internal class RefreshHostedService : IHostedService
    {
        private readonly ILogger logger;

        private readonly IServiceProvider services;

        public RefreshHostedService(IServiceProvider services, ILogger<RefreshHostedService> logger)
        {
            this.services = services;
            this.logger = logger;
        }        

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

                using (IServiceScope scope = services.CreateScope())
                {
                    ISystemTaskService taskService = scope.ServiceProvider.GetRequiredService<ISystemTaskService>();
                    VisionDbContext context = scope.ServiceProvider.GetRequiredService<VisionDbContext>();

                    IEnumerable<SystemTask> tasks = await context.Tasks.OrderByDescending(t => t.Created).Where(t => t.Completed == null).ToListAsync();

                    foreach(var task in tasks)
                    {
                        try
                        {
                            task.Started = DateTime.Now;
                            context.Tasks.Update(task);
                            await context.SaveChangesAsync();                            

                            using (IDbContextTransaction transaction = await context.Database.BeginTransactionAsync())
                            {
                                try
                                {
                                    logger.LogInformation($"Starting Transaction task {task.Scope.ToString()}:{task.Id} processing...");

                                    switch (task.Scope)
                                    {
                                        case TaskScopeKind.Asset:
                                            {
                                                await taskService.RefreshAssetByIdAsync(task.TargetId);
                                                break;
                                            }
                                        case TaskScopeKind.Dependency:
                                            {
                                                await taskService.RefreshDependencyByIdAsync(task.TargetId);
                                                break;
                                            }
                                        case TaskScopeKind.Repository:
                                            {
                                                await taskService.RefreshRepositoryByIdAsync(task.TargetId);
                                                break;
                                            }
                                        case TaskScopeKind.VersionControl:
                                            {
                                                await taskService.RefreshVersionControlByIdAsync(task.TargetId);
                                                break;
                                            }
                                        default:
                                            break;
                                    }

                                    task.Completed = DateTime.Now;
                                    context.Tasks.Update(task);
                                    await context.SaveChangesAsync();

                                    transaction.Commit();
                                }
                                catch (Exception e)
                                {
                                    logger.LogError(e, $"Error performing task. Transaction not commited.");
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            logger.LogError(e, $"Error performing refresh task for {task.Scope.ToString()}:{task.TargetId}");
                        }
                        finally
                        {
                            
                        }
                    }
                }

                await Task.Delay(TimeSpan.FromSeconds(5));
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
            workItems.TryDequeue(out Func<CancellationToken, Task> workItem);
            return workItem;
        }
    }
}
