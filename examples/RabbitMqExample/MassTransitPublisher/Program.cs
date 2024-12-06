using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Models;
using Models.OrangeButton;
using RabbitMQ.Client;
using RabbitMQ.Client.OAuth2;

var authclient = new OAuth2ClientBuilder("orange-bus-publisher", "7e1b6166-2922-4de4-846f-912fb2f554f2", new Uri("https://identitytesteurac.goantares.uno/connect/token"))
    .SetScope(string.Join(" ", [
        "orange-bus.write:orange/topics/omissue",
        "orange-bus.configure:orange/topics/*",
    ]))
    .Build();

var token = authclient.RequestToken();

var authprovider = new OAuth2ClientCredentialsProvider("oauth2", authclient);

var services = new ServiceCollection()
    .AddMassTransit(x =>
    {

        x.UsingRabbitMq((context, cfg) =>
        {

            cfg.UseNewtonsoftRawJsonSerializer();

            cfg.Host("192.168.253.110", "orange", "PublisherExample", h =>
            {
                h.CredentialsProvider = authprovider;
                h.CredentialsRefresher = new MassTransitPublisher.NoOpCredentialsRefresher();
            });

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

            cfg.ConfigureEndpoints(context);
        });
    });


services.AddLogging(logging => {
    logging.SetMinimumLevel(LogLevel.Trace);
    logging.AddConsole();
});

var serviceProvider = services.BuildServiceProvider();

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

var topology = bus.Topology;

await bus.Publish(newOmIssue);
Console.WriteLine($"[{newOmIssue.Data}] Message sent");