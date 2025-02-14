using EuracMonitoringService.Workers.Hooks;
using Microsoft.Extensions.Logging;
using Supernova.Models.GatewayHooks;

namespace EuracMonitoringService.Worker;

public class QueuedHostedService : BackgroundService
{
    private Guid Instance = Guid.NewGuid();
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

        logger.LogInformation($"{nameof(QueuedHostedService)}:{Instance} is stared.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var workItem = await taskQueue.DequeueHookAsync(stoppingToken);

                logger.LogInformation($"{Instance}: Processing hook {workItem.HookName} ({workItem.Id}).");

                if (workItem.HookName != "production-data")
                {
                    logger.LogWarning($"{Instance}: Hook {workItem.HookName} is not supported.");
                    continue;
                }

                logger.LogInformation($"{Instance}: Executing hook {workItem.HookName} ({workItem.Id}).");

                var result = await serviceProvider.GetRequiredService<ProductionDataHook>()
                    .Execute(workItem);

                logger.LogInformation($"{Instance}: Hook {workItem.HookName} ({workItem.Id}) executed.");

                await taskQueue.CompleteHookAsync(workItem, result, stoppingToken);

                //publish job end
            }
            catch (OperationCanceledException ex)
            {
                // Prevent throwing if stoppingToken was signaled
                logger.LogInformation(ex, $"{Instance}: Operation canceled.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"{Instance}:Error occurred executing task work item.");
                //public result in rabbitmq 
            }
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation($"{nameof(QueuedHostedService)}:{Instance} is stopping.");

        await base.StopAsync(stoppingToken);
    }
}
