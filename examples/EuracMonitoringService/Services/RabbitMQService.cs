using Newtonsoft.Json;
using RabbitMQ.Client;
using Supernova.Models.GatewayHooks;
using System.Text;

namespace EuracMonitoringService.Services;

public class RabbitMQService(ConnectionFactory connectionFactory) : IDisposable
{
    private IConnection connection;
    private IModel channel;
    private bool initialize;

    public void Dispose()
    {
        connection.Dispose();
    }

    public ValueTask Init()
    {
        lock (this)
        {
            if (!(this.connection?.IsOpen ?? false))
            {
                this.connection?.Dispose();
                this.connection = connectionFactory.CreateConnection();
            }

            if(this.channel?.IsClosed ?? true)
            {
                this.channel?.Dispose();
                this.channel = connection.CreateModel();
            }
        }

        return ValueTask.CompletedTask;
    }

    public async ValueTask PublishAsync<T>(string exchange, string routingKey, T message)
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

}
