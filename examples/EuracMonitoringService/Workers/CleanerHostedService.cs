//using EuracMonitoringService.Workers.Hooks;

//namespace EuracMonitoringService.Worker;

//public class CleanerHostedService : BackgroundService
//{
//    private IBackgroundHookQueue taskQueue;
//    private ILogger<CleanerHostedService> logger;
//    private readonly IServiceProvider serviceProvider;

//    public CleanerHostedService(IServiceProvider serviceProvider)
//    {
//        this.logger = serviceProvider.GetRequiredService<ILogger<CleanerHostedService>>();
//        this.taskQueue = serviceProvider.GetRequiredService<IBackgroundHookQueue>();
//        this.serviceProvider = serviceProvider;
//    }

//    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//    {
//        await Task.Yield();

//        //TODO
//    }

//    public override async Task StopAsync(CancellationToken stoppingToken)
//    {
//        logger.LogInformation($"{nameof(QueuedHostedService)} is stopping.");

//        await base.StopAsync(stoppingToken);
//    }
//}