using Microsoft.Extensions.Hosting;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using RabbitMQ.Client.OAuth2;
using System.Text;
using Subscriber.Handlers;

var builder = Host.CreateApplicationBuilder(args);
var services = builder.Services;

var authclient = new OAuth2ClientBuilder("orange-bus-subscriber", "95fe531e-2c5a-49e5-82b2-42a5c3a7bda6", new Uri("https://identitytesteurac.goantares.uno/connect/token"))
    .SetScope(string.Join(" ", [
        "orange-bus.read:orange/topics/omissue",
        "orange-bus.read:orange/subscriber-example/*",
        "orange-bus.write:orange/subscriber-example/*",
        "orange-bus.configure:orange/subscriber-example/*",
    ]))
    .Build();

var token = authclient.RequestToken();

var authprovider = new OAuth2ClientCredentialsProvider("orange", authclient);

var factory = new ConnectionFactory()
{
    HostName = "192.168.253.110",
    VirtualHost = "orange",
    ClientProvidedName = "SubscriberExample",
    CredentialsProvider = authprovider
};
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare(queue: "subscriber-example",
    durable: true,
    exclusive: false,
    autoDelete: false,
    arguments: null);

channel.QueueBind("subscriber-example", "topics", "omissue");

var consumer = new EventingBasicConsumer(channel);
consumer.Received += async (ch, ea) =>
{
    await Task.CompletedTask;
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    OMIssueHandler.Handle(message);
};

channel.BasicConsume(queue: "subscriber-example", autoAck: true, consumer: consumer);


var app = builder.Build();
app.Run();