namespace Vision.Web.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Storage;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

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

            while(!cancellationToken.IsCancellationRequested)
            {  
                using (IServiceScope scope = services.CreateScope())
                {
                    IRefreshService taskService = scope.ServiceProvider.GetRequiredService<IRefreshService>();
                    VisionDbContext context = scope.ServiceProvider.GetRequiredService<VisionDbContext>();

                    await HandleRefreshTasks(taskService, context);
                    await HandleRepositoryCleaning(context);
                }

                await Task.Delay(TimeSpan.FromSeconds(10));
            }
        }

        private async Task HandleRepositoryCleaning(VisionDbContext context)
        {
            foreach (var repository in await context.Repositories.Where(repository => repository.IsIgnored).ToListAsync())
            {
                if (await context.Assets.AnyAsync(a => a.RepositoryId == repository.Id))
                {
                    logger.LogInformation($"Removing repository assocated  from repository {repository.Id}");
                    context.AssetFrameworks.RemoveRange(context.AssetFrameworks.Where(af => af.Asset.Repository == repository));
                    context.Assets.RemoveRange(context.Assets.Where(asset => asset.RepositoryId == repository.Id));                    
                    await context.SaveChangesAsync();
                }
            }
        }

        private async Task HandleRefreshTasks(IRefreshService taskService, VisionDbContext context)
        {
            IEnumerable<RefreshTask> tasks = await context.Tasks.OrderByDescending(t => t.Created).Where(t => t.Completed == null).ToListAsync();

            foreach (var task in tasks)
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
                            logger.LogInformation($"Starting refresh task {task.Scope.ToString()}:{task.Id}");

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
                            logger.LogError(e, $"Error performing refresh task {task.Scope.ToString()}:{task.Id}. Transaction not commited.");
                            transaction.Rollback();
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

        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Consume Scoped Service Hosted Service is stopping.");

            return Task.CompletedTask;
        }
    }
}
