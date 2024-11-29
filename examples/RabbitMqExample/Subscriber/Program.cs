using Microsoft.Extensions.Hosting;
using Models;
using Models.OrangeButton;
using Rebus.Config;
using RebusExtensions;
using Subscriber.Handlers;

var vhost = "orange";
var username = "orange";
var password = "orange";
var queueName = "subscriber-example";

var builder = Host.CreateApplicationBuilder(args);
var services = builder.Services;

services.AddRebus(conf =>
    conf.MapTypes(t =>
    {
        t.Map<Message<OMIssue>>($"omissue");
    })
    .Transport(t =>
    {
        t.UseRabbitMq($"amqps://{username}:{password}@192.168.253.110/{vhost}", queueName)
            .Ssl(new Rebus.RabbitMq.SslSettings(false, ""))
            .ClientConnectionName("SubscriberExample");
    }),
    onCreated: async bus =>
    {
        await bus.Subscribe<Message<OMIssue>>();
    },
    isDefaultBus: true
);

services.AddRebusHandler<OMIssueHandler>();

var app = builder.Build();
app.Run();