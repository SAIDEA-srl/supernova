using Microsoft.Extensions.Hosting;
using Models;
using Models.OrangeButton;
using Newtonsoft.Json;
using Rebus.Config;
using Rebus.Serialization;
using Rebus.Serialization.Custom;
using Rebus.Serialization.Json;
using Rebus.Topic;
using Subscriber;
using Subscriber.Handlers;

var vhost = "orange";
var username = "orange";
var password = "orange";
var queueName = "subscriber-example";

var builder = Host.CreateApplicationBuilder(args);
var services = builder.Services;

services.AddRebus(conf =>
    conf.Serialization((s) =>
    {
        s.UseNewtonsoftJson(new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.None
        });
        s.UseCustomMessageTypeNames().AddWithCustomName<Message<OMIssue>>("omissue");
    })
    .Options(o =>
    {
        o.Decorate<ITopicNameConvention>(c =>
        {
            var messageTypeNameConvention = c.Get<IMessageTypeNameConvention>();
            return new TopicNameConvention(messageTypeNameConvention);
        });
    })
    .Transport(t =>
    {
        t.UseRabbitMq($"amqps://{username}:{password}@192.168.253.110/{vhost}", queueName)
            .ExchangeNames("directs", "topics")
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