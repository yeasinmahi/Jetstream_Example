# NATS JetStream Tutorial: Publisher and Dynamic Consumer in C#

This tutorial guides you through setting up a basic NATS JetStream publisher and a dynamic consumer in C#.

## Prerequisites

- .NET Core SDK (version 3.1 or later)
- NATS Server with JetStream enabled
- NATS.Client NuGet package

## Installation

1. Install the .NET Core SDK from the [official website](https://dotnet.microsoft.com/download).

2. Install the NATS.Client package:
    ```bash
    dotnet add package NATS.Client
    ```

3. Ensure you have a running NATS server with JetStream enabled. You can download and run the NATS server from [nats.io](https://nats.io/download/).

## Setting Up the JetStream Launcher application

Create a new .NET console application for the application lancher:

```bash
dotnet new console -n JetstreamExample
cd JetstreamExample
dotnet add package NATS.Client
```

Create a Program.cs file with the following code:
```bash
using System.Diagnostics;

public class Program
{

    public static void Main(string[] args)
    {
        Console.Write("Enter the number of consumers: ");
        if (int.TryParse(Console.ReadLine(), out int numberOfConsumers))
        {
            
            for (int i = 0; i < numberOfConsumers; i++)
            {
                string consumerName = $"Consumer_{i + 1}";
                StartConsumer(consumerName);
            }
        }
        else
        {
            Console.WriteLine("Invalid number.");
        }

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

    

    static void StartConsumer(string consumerName)
    {
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"run --project ../../../../Consumer/Consumer.csproj {consumerName}",
            CreateNoWindow = false,
            UseShellExecute = true
        };

        Process process = new Process
        {
            StartInfo = startInfo
        };

        process.Start();
    }
}


```

## Setting Up the Dynamic Consumer
Create a new .NET console application for the consumer:
```bash
dotnet new console -n Consumer
cd Consumer
dotnet add package NATS.Client
```
Create a Consumer.cs file with the following code:
```bash
using NATS.Client.JetStream;
using NATS.Client;

namespace Consumer
{
    public class Consumer
    {
        public static void consumer(string[] args)
        {
            string natsUrl = "nats://localhost:4222"; // Replace with your NATS server URL
            string stream = "example_stream"; // Replace with your stream name
            string subject = "example.subject"; // Replace with your subject
            string consumerName = args.Length > 0 ? args[0] : "Consumer";

            Console.WriteLine($"{consumerName} is starting...");

            Options opts = ConnectionFactory.GetDefaultOptions();
            opts.Url = natsUrl;

            using (IConnection connection = new ConnectionFactory().CreateConnection(opts))
            {
                IJetStream js = connection.CreateJetStreamContext();

                PushSubscribeOptions pso = PushSubscribeOptions.Builder()
                    .WithStream(stream)
                    .Build();

                EventHandler<MsgHandlerEventArgs> msgHandler = (sender, args) =>
                {
                    Console.WriteLine($"{consumerName} received: {args.Message}");
                    args.Message.Ack();
                };

                IJetStreamPushAsyncSubscription subscription = js.PushSubscribeAsync(subject, msgHandler, false, pso);

                Console.WriteLine($"{consumerName} is listening on {subject}");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }
    }
}

```
Also create a Program.cs file with the following code:
```bash
Consumer.Consumer.consumer(args);
```

## Setting Up the Publisher
Create a new .NET console application for the Publisher:
```bash
dotnet new console -n Publisher
cd Publisher
dotnet add package NATS.Client
```
Create a Publisher.cs file with the following code:
```bash
using NATS.Client;
using NATS.Client.JetStream;

namespace Publisher
{
    public class Publisher
    {
        public static void publisher()
        {
            string natsUrl = "nats://localhost:4222"; // Replace with your NATS server URL
            string stream = "example_stream"; // Replace with your stream name
            string subject = "example.subject"; // Replace with your subject

            Options opts = ConnectionFactory.GetDefaultOptions();
            opts.Url = natsUrl;
            opts.Timeout = 5000; // Set a timeout of 5 seconds

            using (IConnection connection = new ConnectionFactory().CreateConnection(opts))
            {
                IJetStreamManagement jsm = connection.CreateJetStreamManagementContext();
                IJetStream js = connection.CreateJetStreamContext();

                // Create a stream if it doesn't exist
                StreamConfiguration streamConfig = StreamConfiguration.Builder()
                    .WithName(stream)
                    .WithSubjects(subject)
                    .Build();

                try
                {
                    jsm.AddStream(streamConfig);
                }
                catch (NATSJetStreamException e)
                {
                    // If the stream already exists, ignore the error
                    if (e.ApiErrorCode != 10058)
                    {
                        throw;
                    }
                }

                Console.WriteLine("Enter messages to publish. Type 'exit' to quit.");
                while (true)
                {
                    string message = Console.ReadLine();
                    if (message.ToLower() == "exit")
                    {
                        break;
                    }

                    Msg msg = new Msg(subject, System.Text.Encoding.UTF8.GetBytes(message));
                    try
                    {
                        js.Publish(msg);
                        Console.WriteLine($"Published message: {message}");
                    }
                    catch (NATSTimeoutException ex)
                    {
                        Console.WriteLine($"Timeout occurred while publishing message: {message}. Error: {ex.Message}");
                        // Implement retry logic if needed
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error occurred while publishing message: {message}. Error: {ex.Message}");
                    }
                }
            }
        }
    }
}

```
Also create a Program.cs file with the following code:
```bash
Publisher.Publisher.publisher();
```
Run the publisher:
```bash
dotnet run
```
>**Please configure the solution startup as multiple startup project and change none to start for the project JetStramexample and Publisher application**
# Conclusion
In this tutorial, you have set up a basic NATS JetStream publisher and a dynamic consumer in C#. The publisher sends a message to a subject, and the dynamic consumer receives and acknowledges the message. You can expand this basic setup to fit more complex scenarios and business requirements.

For more information on NATS JetStream, refer to the [official documentation](https://docs.nats.io/).
