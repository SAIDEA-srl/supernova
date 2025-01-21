using EuracMonitoringService.Workers.Hooks;
using Microsoft.Extensions.Logging;
using Supernova.Models.GatewayHooks;

namespace EuracMonitoringService.Worker;

public class QueuedHostedService : BackgroundService
{
    private IBackgroundHookQueue taskQueue;
    private ILogger<QueuedHostedService> logger;
    private readonly IServiceProvider serviceProvider;

    public QueuedHostedService(IServiceProvider serviceProvider)
    {
        this.logger = serviceProvider.GetRequiredService<ILogger<QueuedHostedService>>();
        this.taskQueue = serviceProvider.GetRequiredService<IBackgroundHookQueue>();
        this.serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var workItem = await taskQueue.DequeueHookAsync(stoppingToken);

                if(workItem.HookName != "production-data")
                {
                    continue;
                }

                var result = await serviceProvider.GetRequiredService<ProductionDataHook>()
                    .Execute(workItem);

                await taskQueue.CompleteHookAsync(workItem, result, stoppingToken);

                //publish job end
            }
            catch (OperationCanceledException)
            {
                // Prevent throwing if stoppingToken was signaled
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred executing task work item.");
                //public result in rabbitmq 
            }
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation($"{nameof(QueuedHostedService)} is stopping.");

        await base.StopAsync(stoppingToken);
    }
}
