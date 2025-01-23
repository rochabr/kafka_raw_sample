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
                { "isRawPayload", "true" }
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
    
    // Parse the CloudEvent wrapper
    using var jsonDoc = JsonDocument.Parse(json);
    var root = jsonDoc.RootElement;
    
    // Extract the data property and deserialize it as Message
    if (root.TryGetProperty("data", out var data))
    {   
        Console.WriteLine("Received message:");
        Console.WriteLine(data.GetRawText());
        var message = JsonSerializer.Deserialize<Message>(data.GetRawText());
        if (message != null)
        {
            Console.WriteLine($"Received message: {message.Id}");
            Console.WriteLine($"Content: {message.Content}");
            Console.WriteLine($"Timestamp: {message.Timestamp}");
        }
    }

    return Results.Ok();
});

app.Run();