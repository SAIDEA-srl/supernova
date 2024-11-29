using Models;
using Models.OrangeButton;
using Newtonsoft.Json;
using Publisher;
using Rebus.Config;
using Rebus.Serialization;
using Rebus.Serialization.Custom;
using Rebus.Serialization.Json;
using Rebus.Topic;

var sourceName = "PublisherExample";
var vhost = "orange";
var username = "orange";
var password = "orange";

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
        t.UseRabbitMqAsOneWayClient($"amqps://{username}:{password}@192.168.253.110/{vhost}")
            .Ssl(new Rebus.RabbitMq.SslSettings(false, ""))
            .ClientConnectionName(sourceName);
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