using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);


var env = builder.AddDockerComposeEnvironment("docker-compose")
    .ConfigureComposeFile(cfg =>
    {
        cfg.AddNetwork(new Aspire.Hosting.Docker.Resources.ComposeNodes.Network()
        {
            Name = "supernova",
            External = true,
            Driver = "bridge",
        });

        cfg.AddNetwork(new Aspire.Hosting.Docker.Resources.ComposeNodes.Network()
        {
            Name = "mongo-network",
            External = false,
        });        
    });

env.Resource.DefaultNetworkName = "mongo-network";

var identityserver = env.ApplicationBuilder.AddParameter("IdentityAuthority", "http://localhost:5005");

var mongo = env.ApplicationBuilder.AddMongoDB("mongo")
        .WithDataVolume("universalmapper-db-volume")
        .WithMongoExpress((options) => {
            options.PublishAsContainer();
        }, "mongo-express");

var mongodb = mongo.AddDatabase("universalmapper-db");

var apiService = env.ApplicationBuilder.AddProject<Projects.UniversalMapper>("universalmapper")
    .WithEnvironment("IdentityAuthority", identityserver)
    .WithExternalHttpEndpoints()
    .WithReference(mongodb)
    .WaitFor(mongodb);


env.ApplicationBuilder.Build().Run();
