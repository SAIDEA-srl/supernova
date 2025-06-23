using RabbitMQ.Client;
using RabbitMQ.Client.OAuth2;

namespace EuracMonitoringService.Services;

public class RabbitMQCredentialRefresher(ConnectionFactory connectionFactory,
        ILogger<RabbitMQCredentialRefresher> logger) : IHostedService
{
    private CredentialsRefresher clientRefresher = null;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (connectionFactory?.CredentialsProvider != null)
        {
            this.clientRefresher = new CredentialsRefresher(connectionFactory.CredentialsProvider, (credentials, exception, cancellationToken) =>
            {
                if (exception != null)
                {
                    logger.LogError(exception, $"Error refreshing credentials: {exception.Message}");
                    return Task.FromException(exception);
                }
                else
                {
                    logger.LogInformation($"Credentials refreshed successfully. {credentials.ValidUntil}");
                }

                return Task.CompletedTask;
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
