var builder = DistributedApplication.CreateBuilder(args);

var mongo = builder.AddMongoDB("mongo")
        .WithDataVolume("universalmapper-db-volume")
        .WithMongoExpress((options) => { }, "mongo-express");

var mongodb = mongo.AddDatabase("universalmapper-db");

var apiService = builder.AddProject<Projects.UniversalMapper>("universalmapper")
    .WithExternalHttpEndpoints()
    .WithReference(mongodb)
    .WaitFor(mongodb);


builder.Build().Run();
