using Models.OrangeButton;
using Models;
using RabbitMQ.Client;
using System.Text;
using Newtonsoft.Json;


var sourceName = "PublisherExample";

var factory = new ConnectionFactory()
{
    HostName = "192.168.253.110",
    VirtualHost = "orange",
    UserName = "orange",
    Password = "orange",
    ClientProvidedName = sourceName
};
using (var connection = await factory.CreateConnectionAsync())
using (var channel = await connection.CreateChannelAsync())
{
    await channel.ExchangeDeclareAsync("RebusTopics", ExchangeType.Topic);

    var deviceId = Guid.NewGuid();
    var newOmIssue = new Message<OMIssue>()
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
    };

    var message = JsonConvert.SerializeObject(newOmIssue);
    var body = Encoding.UTF8.GetBytes(message);
    await channel.BasicPublishAsync("RebusTopics", "omissue", body);
}
