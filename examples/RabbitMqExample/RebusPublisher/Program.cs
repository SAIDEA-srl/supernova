using Models;
using Models.OrangeButton;
using Newtonsoft.Json;
using Publisher;
using Rebus.Config;
using Rebus.Serialization;
using Rebus.Serialization.Custom;
using Rebus.Serialization.Json;
using Rebus.Topic;
using RabbitMQ.Client.OAuth2;

var sourceName = "PublisherExample";
var vhost = "orange";

var bus = Configure.OneWayClient()
    .Serialization((s) =>
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
        t.UseRabbitMqAsOneWayClient($"amqps://192.168.253.110/{vhost}")
            .ExchangeNames("directs", "topics")
            .Ssl(new Rebus.RabbitMq.SslSettings(false, ""))
            .Declarations(declareExchanges: false)
            .CustomizeConnectionFactory(factory =>
            {
                var scoppes = string.Join(" ", [
                        "orange-bus.write:orange/topics/omissue",
                        "orange-bus.read:orange/directs/*",
                        "orange-bus.write:orange/directs/*",
                        "orange-bus.write:orange/error/*",
                        "orange-bus.configure:orange/error/*"
                    ]);

                var authclient = new OAuth2ClientBuilder("orange-bus-publisher", "7e1b6166-2922-4de4-846f-912fb2f554f2", new Uri("https://identitytesteurac.goantares.uno/connect/token"))
                    .SetScope(scoppes)
                    .Build();

                var token = authclient.RequestToken();

                factory.ClientProvidedName = sourceName;
                factory.CredentialsProvider = new OAuth2ClientCredentialsProvider("orange", authclient);
                return factory;
            });
})
    .Start();

var deviceId = Guid.NewGuid();

await bus.Publish(new Message<OMIssue>()
{
    DateTime = DateTime.Now,
    Source = sourceName,
    Data = new OMIssue()
    {
        IssueUUID = new IssueUUID() { Value = Guid.NewGuid().ToString() },
        IssueID = new IssueID() { Value = $"1" },
        IssueStatus = new IssueStatus() { Value = "Open" },
        Description = new Description() { Value = $"A new issue example" },
        IssueFoundDate = new IssueFoundDate() { Value = DateTimeOffset.Now.ToString("u") },
        Scope = new Scope()
        {
            ScopeID = new ScopeID() { Value = deviceId.ToString() },
        }
    }
});