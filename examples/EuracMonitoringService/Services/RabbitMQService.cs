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

    private SemaphoreSlim semaphore = new(1);

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
                    currentconnection.Close();
                    currentconnection.Dispose();
                }
                catch(Exception ex)
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

    public async ValueTask<IModel> Init()
    {

        var currentConnection = this.connection;

        if (currentConnection != null && currentConnection.IsOpen)
        {
            return currentConnection.CreateModel();
        }


        await semaphore.WaitAsync();

        try
        {
            currentConnection = this.connection;

            if (currentConnection != null)
            {
                if (currentConnection.IsOpen)
                {
                    return currentConnection.CreateModel();
                }

                logger.LogInformation("Existing connection found to be CLOSED");

                try
                {
                    currentConnection.Close();
                    currentConnection.Dispose();
                }
                catch (Exception e)
                {
                    logger.LogDebug($"Exception thrown while closing and disposing connection. This can probably be ignored, but for good measure, here it is: {e.Message}");
                }
            }

            try
            {
                connection = connectionFactory.CreateConnection();

                return connection.CreateModel();
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
        try
        {
            using var channel = await Init();

            var serialized = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(serialized);

            var props = channel.CreateBasicProperties();
            props.MessageId = Guid.NewGuid().ToString();
            props.ContentType = "application/json";
            props.ContentEncoding = "utf-8";
            props.Type = routingKey;

            channel.BasicPublish(exchange, routingKey, true, props, body);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
        }
    }

}
