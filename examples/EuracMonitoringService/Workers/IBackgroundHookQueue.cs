using Supernova.Models.GatewayHooks;

namespace EuracMonitoringService.Worker;

public interface IBackgroundHookQueue
{
    GatewayHookExecution? GetHookExecution(Guid id);

    ValueTask QueueHookAsync(GatewayHookExecution workItem);

    ValueTask<GatewayHookExecution> DequeueHookAsync(CancellationToken cancellationToken);

    ValueTask CompleteHookAsync(GatewayHookExecution exec, GatewayHookResult result, CancellationToken cancellationToken);
    
}
