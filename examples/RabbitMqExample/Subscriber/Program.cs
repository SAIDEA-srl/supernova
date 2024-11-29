using Microsoft.Extensions.Hosting;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using Subscriber.Handlers;

var builder = Host.CreateApplicationBuilder(args);
var services = builder.Services;

var factory = new ConnectionFactory()
{
    HostName = "192.168.253.110",
    VirtualHost = "orange",
    UserName = "orange",
    Password = "orange",
    ClientProvidedName = "SubscriberExample"
};
using (var connection = await factory.CreateConnectionAsync())
using (var channel = await connection.CreateChannelAsync())
{
    await channel.QueueDeclareAsync(queue: "subscriber-example",
                         durable: true,
                         exclusive: false,
                         autoDelete: false,
                         arguments: null);

    await channel.QueueBindAsync("subscriber-example", "RebusTopics", "omissue");

    var consumer = new AsyncEventingBasicConsumer(channel);
    consumer.ReceivedAsync += async (model, ea) =>
    {
        await Task.CompletedTask;
        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        OMIssueHandler.Handle(message);
    };
    await channel.BasicConsumeAsync(queue: "subscriber-example", autoAck: true, consumer: consumer);
}

var app = builder.Build();
app.Run();