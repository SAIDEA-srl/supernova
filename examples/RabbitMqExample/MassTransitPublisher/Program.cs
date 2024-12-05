using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Models;
using Models.OrangeButton;
using RabbitMQ.Client;
using RabbitMQ.Client.OAuth2;


var authclient = new OAuth2ClientBuilder("orange-bus-publisher", "7e1b6166-2922-4de4-846f-912fb2f554f2", new Uri("https://identitytesteurac.goantares.uno/connect/token"))
    .SetScope(string.Join(" ", [
        "orange-bus.write:orange/topics/omissue",
    ]))
    .Build();

var authprovider = new OAuth2ClientCredentialsProvider("oauth2", authclient);

var builder = Host.CreateApplicationBuilder(args);
var services = builder.Services;

var collection = new ServiceCollection()
    .AddMassTransit(x =>
    {
        x.UsingRabbitMq((context, cfg) =>
        {
            cfg.UseNewtonsoftRawJsonSerializer();
            cfg.Host("192.168.253.110", "orange", "PublisherExample", h =>
            {
                h.CredentialsProvider = authprovider;
            });
            cfg.ConfigureEndpoints(context);

            cfg.Send<Message<OMIssue>>(x =>
            {
                x.UseRoutingKeyFormatter(context => "omissue");
            });
            cfg.Message<Message<OMIssue>>(x =>
            {
                x.SetEntityName("topics");
            });
            cfg.Publish<Message<OMIssue>>(x =>
            {
                x.ExchangeType = ExchangeType.Topic;
            });
        });
    });
var serviceProvider = collection.BuildServiceProvider();

var bus = serviceProvider.GetRequiredService<IBus>();

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

using var source = new CancellationTokenSource(TimeSpan.FromSeconds(30));
await bus.Publish(newOmIssue, source.Token);
Console.WriteLine($"[{newOmIssue.Data}] Message sent");