using Newtonsoft.Json;
using RabbitMQ.Client;
using Supernova.Models.GatewayHooks;
using System.Text;

namespace EuracMonitoringService.Services;

public class RabbitMQService(ConnectionFactory connectionFactory, ILogger<RabbitMQService> logger) : IDisposable
{

    private SemaphoreSlim semaphore = new(1);

    private IConnection connection;
    private IModel channel;

    public void Dispose()
    {
        connection.Dispose();
    }

    public async ValueTask Init()
    {
        await semaphore.WaitAsync();

        try
        {
            if (!(this.connection?.IsOpen ?? false))
            {
                this.connection?.Dispose();
                this.connection = connectionFactory.CreateConnection();
            }

            if (this.channel?.IsClosed ?? true)
            {
                this.channel?.Dispose();
                this.channel = connection.CreateModel();
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
            await Init();

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
