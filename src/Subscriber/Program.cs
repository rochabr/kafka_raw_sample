using System.Text.Json;
using Shared;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Subscriber API");
// Subscription endpoint that Dapr will call to get the subscription configuration
app.MapGet("/dapr/subscribe", () =>
{
    var subscriptions = new[]
    {
        new
        {
            pubsubname = "pubsub",
            topic = "messages",
            route = "/messages",
            metadata = new Dictionary<string, string>
            {
                { "isRawPayload", "true" },
                { "content-type", "application/json" }
            }
        }
    };
    return Results.Ok(subscriptions);
});

// Message handler endpoint
app.MapPost("/messages", async (HttpContext context) =>
{
    using var reader = new StreamReader(context.Request.Body);
    var json = await reader.ReadToEndAsync();
    
    // Parse the event
    var jsonDoc = JsonDocument.Parse(json);
    
    // Deserialize it as Message
    var message = JsonSerializer.Deserialize<Message>(jsonDoc);
    if (message != null)
    {
        Console.WriteLine($"Received message: {message.Id}");
        Console.WriteLine($"Content: {message.Content}");
        Console.WriteLine($"Timestamp: {message.Timestamp}");
    }


    return Results.Ok();
});

app.Run();