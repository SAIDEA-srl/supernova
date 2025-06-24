using RabbitMQ.Client;
using RabbitMQ.Client.OAuth2;

namespace EuracMonitoringService.Services;

public class RabbitMQCredentialRefresher(
        ConnectionFactory connectionFactory,
        RabbitMQService rabbitMQService,
        ILogger<RabbitMQCredentialRefresher> logger) : IHostedService
{
    private CredentialsRefresher clientRefresher = null;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (connectionFactory?.CredentialsProvider != null)
        {
            this.clientRefresher = new CredentialsRefresher(connectionFactory.CredentialsProvider, async (credentials, exception, cancellationToken) =>
            {
                if (exception != null)
                {
                    logger.LogError(exception, $"Error refreshing credentials: {exception.Message}");
                }
                else
                {
                    await rabbitMQService.UpdateCredential(credentials);
                    logger.LogInformation($"Credentials refreshed successfully. {DateTime.Now.Add(credentials.ValidUntil ?? TimeSpan.Zero)}");
                }
            }, cancellationToken);
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        this.clientRefresher?.Dispose();
        return Task.CompletedTask;
    }
}
