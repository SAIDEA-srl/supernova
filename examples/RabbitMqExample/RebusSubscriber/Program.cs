using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Models;
using Models.OrangeButton;
using Newtonsoft.Json;
using Rebus.Config;
using Rebus.Logging;
using Rebus.RabbitMq;
using Rebus.Serialization;
using Rebus.Serialization.Custom;
using Rebus.Serialization.Json;
using Rebus.Topic;
using RebusSubscriber.Extensions;
using RabbitMQ.Client.OAuth2;
using Subscriber;
using Subscriber.Handlers;
using System.Net;

var vhost = "orange";
var queueName = "subscriber-example";

var builder = Host.CreateApplicationBuilder(args);
var services = builder.Services;

services.AddRebus(conf =>
    conf.Serialization((s) =>
    {
        s.UseNewtonsoftJson(new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.None,
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

        var options = t.UseRabbitMq($"amqps://192.168.253.110/{vhost}", queueName)
            .ExchangeNames("directs", "topics")
            .Declarations(declareExchanges: false)
            .Ssl(new Rebus.RabbitMq.SslSettings(false, ""))
            .CustomizeConnectionFactory(factory =>
            {

                var scoppes = string.Join(" ", [
                        "orange-bus.read:orange/topics/omissue",
                        "orange-bus.read:orange/directs/*",
                        "orange-bus.write:orange/directs/*",
                        $"orange-bus.read:orange/{queueName}/*",
                        $"orange-bus.write:orange/{queueName}/*",
                        $"orange-bus.configure:orange/{queueName}/*",
                        $"orange-bus.write:orange/error/*",
                        $"orange-bus.configure:orange/error/*"
                    ]);

                var authclient = new OAuth2ClientBuilder("orange-bus-subscriber", "95fe531e-2c5a-49e5-82b2-42a5c3a7bda6", new Uri("https://identitytesteurac.goantares.uno/connect/token"))
                    .SetScope(scoppes)
                    .Build();

                var token = authclient.RequestToken();

                factory.ClientProvidedName = "SubscriberExample";
                factory.CredentialsProvider = new OAuth2ClientCredentialsProvider("orange", authclient);
                return factory;
            });


        t.Decorate(sp => new CustomRabbitMqTransport(sp.Get<RabbitMqTransport>()));

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