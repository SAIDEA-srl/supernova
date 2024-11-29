using Models;
using Models.OrangeButton;
using Rebus.Config;
using Rebus.Messages;
using RebusExtensions;

var sourceName = "PublisherExample";
var vhost = "orange";
var username = "orange";
var password = "orange";

var bus = Configure.OneWayClient()
    .MapTypes(t =>
    {
        t.Map<Message<OMIssue>>($"omissue");
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