using Models.OrangeButton;
using Models;
using RabbitMQ.Client;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client.OAuth2;

var authclient = new OAuth2ClientBuilder("orange-bus-publisher", "7e1b6166-2922-4de4-846f-912fb2f554f2", new Uri("https://identitytesteurac.goantares.uno/connect/token"))
    .SetScope(string.Join(" ", [
        "orange-bus.write:orange/topics/omissue",
    ]))
    .Build();

var authprovider = new OAuth2ClientCredentialsProvider("oauth2", authclient);

var factory = new ConnectionFactory()
{
    HostName = "192.168.253.110",
    VirtualHost = "orange",
    ClientProvidedName = "PublisherExample",
    CredentialsProvider = authprovider
};
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

//exchange must be configured
//channel.ExchangeDeclare("topics", ExchangeType.Topic);

var deviceId = Guid.NewGuid();
var newOmIssue = new Message<OMIssue>()
{
    DateTime = DateTime.Now,
    Source = "PublisherExample",
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

var props = channel.CreateBasicProperties();
props.MessageId = Guid.NewGuid().ToString();
props.ContentType = "application/json";
props.ContentEncoding = "utf-8";

//Questi header sono per l'interoperabilita con rebus
props.Headers = new Dictionary<string, object>
{
    ["rbs2-content-type"] = "application/json",
    ["rbs2-msg-type"] = "omissue"
};

channel.BasicPublish("topics", "omissue", true, props, body);
Console.WriteLine($"[{newOmIssue.Data}] Message sent");
