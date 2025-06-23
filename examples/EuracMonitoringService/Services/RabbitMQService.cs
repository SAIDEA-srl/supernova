using Microsoft.AspNetCore.Connections;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Serilog;
using Supernova.Models.GatewayHooks;
using System.Text;
using System.Threading;

namespace EuracMonitoringService.Services;

public class RabbitMQService(ConnectionFactory connectionFactory, ILogger<RabbitMQService> logger) : IAsyncDisposable
{

    private static SemaphoreSlim semaphore = new(1);

    private IConnection? connection = null;

    public async ValueTask DisposeAsync()
    {
        await semaphore.WaitAsync();
        try
        {
            var currentconnection = this.connection;

            if (currentconnection != null)
            {
                logger.LogInformation("Disposing RabbitMQ connection");

                try
                {
                    await currentconnection.CloseAsync();
                    currentconnection.Dispose();
                }
                catch (Exception ex)
                {
                    logger.LogDebug(ex, $"Exception thrown while closing and disposing connection. This can probably be ignored, but for good measure, here it is: {ex.Message}");
                }
                finally
                {
                    connection = null;
                }
            }
        }
        finally
        {
            semaphore.Release();
        }
    }

    public async ValueTask<IChannel> Init()
    {

        var currentConnection = this.connection;

        if (currentConnection != null && currentConnection.IsOpen)
        {
            return await currentConnection.CreateChannelAsync();
        }


        await semaphore.WaitAsync();

        try
        {
            currentConnection = this.connection;

            if (currentConnection != null)
            {
                if (currentConnection.IsOpen)
                {
                    return await currentConnection.CreateChannelAsync();
                }

                logger.LogInformation("Existing connection found to be CLOSED");

                try
                {
                    await currentConnection.CloseAsync();
                    currentConnection.Dispose();
                }
                catch (Exception e)
                {
                    logger.LogDebug($"Exception thrown while closing and disposing connection. This can probably be ignored, but for good measure, here it is: {e.Message}");
                }
            }

            try
            {
                connection = await connectionFactory.CreateConnectionAsync();
                return await connection.CreateChannelAsync();
            }
            catch (Exception exception)
            {
                logger.LogWarning($"Could not establish connection: {exception.Message}");
                await Task.Delay(1000); // if CreateConnection fails fast for some reason, we wait a little while here to avoid thrashing tightly
                throw;
            }
        }
        finally
        {
            semaphore.Release();
        }
    }

    public async ValueTask PublishAsync<T>(string exchange, string routingKey, T message)
    {
        using var channel = await Init();

        var serialized = JsonConvert.SerializeObject(message);
        var body = Encoding.UTF8.GetBytes(serialized);

        var props = new BasicProperties();
        props.MessageId = Guid.NewGuid().ToString();
        props.ContentType = "application/json";
        props.ContentEncoding = "utf-8";
        props.Type = routingKey;

        await channel.BasicPublishAsync(exchange, routingKey, true, props, body);
    }

}
