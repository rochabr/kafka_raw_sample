using Dapr.Client;
using Shared;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers().AddDapr();

var app = builder.Build();

app.MapGet("/", () => "Publisher API");

app.MapPost("/publish", async (DaprClient daprClient) =>
{
    var message = new Message(
        Guid.NewGuid().ToString(),
        $"Hello at {DateTime.UtcNow}",
        DateTime.UtcNow
    );

    await daprClient.PublishEventAsync(
        "pubsub",           // pubsub name
        "messages",         // topic name
        message,           // message data
        new Dictionary<string, string> 
        { 
            { "rawPayload", "true" },
            { "content-type", "application/json" }
        }
    );
    
    return Results.Ok(message);
});

app.Run();