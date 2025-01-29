using Microsoft.Extensions.Caching.Memory;
using System.Threading.Channels;
using Supernova.Models.GatewayHooks;
using RabbitMQ.Client;
using EuracMonitoringService.Services;
using Supernova.Messages.Models;

namespace EuracMonitoringService.Worker;

public sealed class BackgroundHookQueue : IBackgroundHookQueue
{
    private readonly IMemoryCache cache;
    private readonly ILogger<BackgroundHookQueue> logger;
    private readonly RabbitMQService rabbitmqService;
    private readonly Channel<GatewayHookExecution> _queue;

    public GatewayHookExecution? GetHookExecution(Guid id)
        => cache?.Get<GatewayHookExecution>(id);

    public BackgroundHookQueue(ILogger<BackgroundHookQueue> logger,
        RabbitMQService rabbitmqService,
        IMemoryCache memoryCache, int capacity)
    {
        this.logger = logger;
        this.rabbitmqService = rabbitmqService;
        cache = memoryCache;

        BoundedChannelOptions options = new(capacity)
        {
            FullMode = BoundedChannelFullMode.Wait
        };

        _queue = Channel.CreateBounded<GatewayHookExecution>(options);        
    }

    public async ValueTask QueueHookAsync(GatewayHookExecution workItem)
    {
        ArgumentNullException.ThrowIfNull(workItem);        

        var memoryCacheEntryOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(DateTimeOffset.Now.AddMinutes(1))
            .SetPriority(CacheItemPriority.NeverRemove)
            .RegisterPostEvictionCallback(DisposeWorkItemResult, this);

        cache.Set(workItem.Id, workItem, memoryCacheEntryOptions);

        await _queue.Writer.WriteAsync(workItem);

        //push job status
        await rabbitmqService.PublishAsync("topics", "supernova.hookcompletion.eurac-monitoring-service", new MessageGatewayHookExecution()
        {
            Data = workItem,
            DateTime = DateTimeOffset.Now,
            Source = "eurac-monitoring-service"
        });

    }

    public async ValueTask<GatewayHookExecution> DequeueHookAsync(CancellationToken cancellationToken)
    {
        return await _queue.Reader.ReadAsync(cancellationToken);
    }


    public async ValueTask CompleteHookAsync(GatewayHookExecution exec, GatewayHookResult result, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(exec);
        ArgumentNullException.ThrowIfNull(result);

        exec.Result = result;
        exec.Status = HookStatus.End;
        //publish job end

        await rabbitmqService.PublishAsync("topics", "supernova.hookcompletion.eurac-monitoring-service", new MessageGatewayHookExecution()
        {
            Data = exec,
            DateTime = DateTimeOffset.Now,
            Source = "eurac-monitoring-service"
        });

    }


    private static void DisposeWorkItemResult(object cacheKey, object cacheValue, EvictionReason evictionReason, object state)
    {
        //TODO rivedere questa cosa
        var backgroundQueue = (BackgroundHookQueue)state;

        if (cacheValue is GatewayHookExecution execution)
        {
            if(execution.Result != null)
            {
                ReportManager.DeleteFile(execution.Result.ResponseUrl);

            }
        }
    }
}