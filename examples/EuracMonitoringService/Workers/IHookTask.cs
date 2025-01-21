using Supernova.Models.GatewayHooks;

namespace EuracMonitoringService.Workers;


public interface IHookTask
{
    public abstract Task<GatewayHookResult> Execute(GatewayHookExecution exec);
}

