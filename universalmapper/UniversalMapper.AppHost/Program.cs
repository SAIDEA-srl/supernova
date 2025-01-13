using Aspire.Hosting;
using Json.Patch;

var builder = DistributedApplication.CreateBuilder(args);

var identityserver = builder.AddParameter("IdentityAuthority", "http://localhost:5005");

var mongo = builder.AddMongoDB("mongo")
        .WithDataVolume("universalmapper-db-volume")
        .WithMongoExpress((options) => {
            options.PublishAsContainer();
        }, "mongo-express");

var mongodb = mongo.AddDatabase("universalmapper-db");

var apiService = builder.AddProject<Projects.UniversalMapper>("universalmapper")
    .WithEnvironment("IdentityAuthority", identityserver)
    .WithExternalHttpEndpoints()
    .WithReference(mongodb)
    .WaitFor(mongodb);


builder.Build().Run();
