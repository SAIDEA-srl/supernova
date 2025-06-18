using EuracMonitoringService.Services;
using EuracMonitoringService.Worker;
using EuracMonitoringService.Workers.Hooks;
using InfluxDB.Client;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using RabbitMQ.Client;
using RabbitMQ.Client.OAuth2;
using Serilog;
using System.Text.Json.Serialization;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddSerilog((services, lc) => lc
    .ReadFrom.Configuration(builder.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext());


// Add services to the container.
builder.Services.AddMemoryCache();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//add bearer tocket authentication
builder.Services.AddAuthentication()
       .AddJwtBearer(options =>
       {
           options.Authority = builder.Configuration["OpenId:Authority"];
           options.Audience = "eurac-monitoring-service";
           options.RequireHttpsMetadata = false;
           options.TokenValidationParameters = new TokenValidationParameters
           {
               ValidateIssuer = true,
               ValidateAudience = true,
               ValidateLifetime = true,
               ValidateIssuerSigningKey = true,
               // The IssuerSigningKey will be automatically retrieved from the OpenID Connect server
           };
       });




//create rabbitmp connection factory
builder.Services.AddSingleton((sp) =>
{
    return new Lazy<Task<ConnectionFactory>>(async () =>
    {
        var authbuilder = new OAuth2ClientBuilder(
            builder.Configuration["RabbitMQ:OAuth:ClientId"],
            builder.Configuration["RabbitMQ:OAuth:ClientSecret"],
            new Uri($"{builder.Configuration["OpenId:Authority"]}/connect/token"));

        var authclient = await authbuilder.BuildAsync();

        var authprovider = new OAuth2ClientCredentialsProvider("supernova", authclient);

        return new ConnectionFactory()
        {
            HostName = builder.Configuration["RabbitMQ:Host"],
            Port = 5672,
            VirtualHost = "supernova",
            ClientProvidedName = builder.Configuration["RabbitMQ:OAuth:ClientId"],
            CredentialsProvider = authprovider,
        };

    });
});

builder.Services.AddSingleton<RabbitMQService>();


builder.Services.AddTransient((sp) =>
{
    var influxUrl = builder.Configuration["InfluxDB:Url"];
    var influxToken = builder.Configuration["InfluxDB:Token"];
    var influxDatabase = builder.Configuration["InfluxDB:Database"];
    var influxOrg = builder.Configuration["InfluxDB:Organization"];

    return InfluxDBClientFactory.Create(InfluxDBClientOptions.Builder.CreateNew()
        .Url(influxUrl)
        .AuthenticateToken(influxToken)
        .Bucket(influxDatabase)
        .Org(influxOrg)
        .VerifySsl(false)
        .Build()
    );

});

builder.Services.AddSingleton<IBackgroundHookQueue>(sp =>
{
    if (!int.TryParse(builder.Configuration["QueueCapacity"], out var queueCapacity))
    {
        queueCapacity = 100;
    }

    return new BackgroundHookQueue(
        sp.GetRequiredService<ILogger<BackgroundHookQueue>>(),
        sp.GetRequiredService<RabbitMQService>(),
        sp.GetRequiredService<IMemoryCache>(),
        queueCapacity);
});

#region HOSTED SERVICES
//add hosted services
if (!int.TryParse(builder.Configuration["QueueProcessors"], out var queueProcessors))
{
    queueProcessors = 1;
}
for (var i = 0; i < queueProcessors; i++)
{
    builder.Services.AddHostedService((sp) => new QueuedHostedService(sp));
}
#endregion


builder.Services.AddTransient<ProductionDataHook>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/synchronous_asset_api.json", "");
});
app.UseHttpsRedirection();

app.MapDefaultEndpoints();

app.UseSerilogRequestLogging();

app.UseAuthorization();
app.UseAuthentication();

if (string.IsNullOrWhiteSpace(builder.Configuration["Reports:BasePath"]))
{
    app.UseStaticFiles();
}
else
{
    Log.Information($"Use document base path: {builder.Configuration["Reports:BasePath"]}");
    app.UseStaticFiles();

    var directory = $"{builder.Configuration["Reports:BasePath"]}";
    Directory.CreateDirectory(directory);

    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(directory),
        RequestPath = "/reports"
    });
}

app.MapControllers();

app.MapFallback((HttpContext context) =>
{
    if (context.Request.Path.StartsWithSegments("/Devices") ||
        context.Request.Path.StartsWithSegments("/OMTasks") ||
        context.Request.Path.StartsWithSegments("/PVArrays") ||
        context.Request.Path.StartsWithSegments("/PVStrings") ||
        context.Request.Path.StartsWithSegments("/PVSystems"))
    {
        context.Response.StatusCode = StatusCodes.Status501NotImplemented;
    }
    else
    {
        context.Response.StatusCode = StatusCodes.Status404NotFound;
    }

    return Task.CompletedTask;
});


app.Run();
