# Dapr PubSub Sample with Kafka

This sample demonstrates how to use Dapr's PubSub API with Kafka in .NET applications. It consists of a publisher and subscriber application communicating through Kafka using raw JSON messages.

## Using Declarative Subscriptions

Instead of using programmatic subscriptions (the `/dapr/subscribe` endpoint), you can use declarative subscriptions. Create a file named `subscription.yaml` in your components directory:

```yaml
apiVersion: dapr.io/v2alpha1
kind: Subscription
metadata:
  name: message-subscription
spec:
  topic: messages
  routes:
    default: /messages
  pubsubname: pubsub
  metadata:
    rawPayload: "true"
```

When using declarative subscriptions:

1. Remove the `/dapr/subscribe` endpoint from your subscriber application
2. Place the `subscription.yaml` file in your components directory
3. The subscription will be automatically loaded when you start your application



This sample demonstrates how to use Dapr's PubSub API with Kafka in .NET applications. It consists of a publisher and subscriber application communicating through Kafka using raw JSON messages.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Dapr CLI](https://docs.dapr.io/getting-started/install-dapr-cli/)
- [Docker](https://www.docker.com/products/docker-desktop)

## Setup

1. Clone the repository
2. Start Kafka using Docker Compose:

```bash
docker-compose up -d
```

## Running the Applications

1. Start the Subscriber:

```bash
dapr run --app-id subscriber \
         --app-port 5001 \
         --dapr-http-port 3501 \
         --resources-path ./components \
         -- dotnet run --project src/Subscriber/Subscriber.csproj
```

2. In a new terminal, start the Publisher:

```bash
dapr run --app-id publisher \
         --app-port 5000 \
         --dapr-http-port 3500 \
         --resources-path ./components \
         -- dotnet run --project src/Publisher/Publisher.csproj
```

## Testing

To publish a message, use curl or any HTTP client:

```bash
curl -X POST http://localhost:5000/publish
```

The subscriber will display received messages in its console output.

## Stopping the Applications

1. Stop the running applications using Ctrl+C in each terminal
2. Stop Kafka:

```bash
docker-compose down
```