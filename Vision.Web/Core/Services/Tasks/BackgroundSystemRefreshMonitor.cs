using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Vision.Web.Core
{
    internal class BackgroundSystemRefreshMonitor : IHostedService
    {
        private readonly ILogger logger;
        private readonly IServiceProvider services;

        public BackgroundSystemRefreshMonitor(IServiceProvider services, ILogger<BackgroundSystemRefreshMonitor> logger)
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
            logger.LogInformation("Hosted Service started.");

            using (IServiceScope scope = services.CreateScope())
            {
                IRefreshService taskService = scope.ServiceProvider.GetRequiredService<IRefreshService>();
                VisionDbContext context = scope.ServiceProvider.GetRequiredService<VisionDbContext>();

                while (!cancellationToken.IsCancellationRequested)
                {
                    using (var transaction = await context.Database.BeginTransactionAsync())
                    {
                        try
                        {
                            await HandleRefreshTasks(taskService, context);
                            await HandleRepositoryCleaning(context);
                            transaction.Commit();
                        }
                        catch (Exception e)
                        {
                            logger.LogCritical(e, "Transaction roll back");
                            transaction.Rollback();
                        }
                    }

                    await Task.Delay(TimeSpan.FromSeconds(10));
                }

                scope.Dispose();
            }
        }

        private async Task HandleRepositoryCleaning(VisionDbContext context)
        {
            foreach (var repository in await context.VcsRepositories.Where(repository => repository.IsIgnored).ToListAsync())
            {
                if (await context.Assets.AnyAsync(a => a.RepositoryId == repository.Id))
                {
                    logger.LogInformation($"Removing repository assocated from repository {repository.Id}");
                    context.AssetEcoSystems.RemoveRange(context.AssetEcoSystems.Where(af => af.Asset.Repository == repository));
                    context.Assets.RemoveRange(context.Assets.Where(asset => asset.RepositoryId == repository.Id));
                    await context.SaveChangesAsync();
                }
            }
        }

        private async Task HandleRefreshTasks(IRefreshService taskService, VisionDbContext context)
        {
            foreach (RefreshTask task in await context.Tasks.OrderByDescending(t => t.Created).Where(t => t.Completed == null).ToListAsync())
            {
                task.Started = DateTime.Now;
                context.Tasks.Update(task);
                await context.SaveChangesAsync();

                try
                {
                    logger.LogInformation($"Starting refresh task {task.Scope.ToString()}:{task.Id}");

                    Task refreshTask = task.Scope switch
                    {
                        TaskScopeKind.Asset => taskService.RefreshAssetByIdAsync(task.TargetId),
                        TaskScopeKind.Dependency => taskService.RefreshDependencyByIdAsync(task.TargetId),
                        TaskScopeKind.Repository => taskService.RefreshRepositoryByIdAsync(task.TargetId),
                        TaskScopeKind.VersionControl => taskService.RefreshVersionControlByIdAsync(task.TargetId),
                        _ => Task.Delay(0)
                    };

                    await refreshTask;

                    task.Completed = DateTime.Now;
                    context.Tasks.Update(task);
                    await context.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    logger.LogError(e, $"Error performing refresh task {task.Scope.ToString()}:{task.Id}.");
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Consume Scoped Service Hosted Service is stopping.");

            return Task.CompletedTask;
        }
    }
}