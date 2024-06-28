# NATS Tutorial: Publisher and Dynamic Consumer in C#

This tutorial guides you through setting up a basic NATS publisher and a dynamic consumer in C#.

## Prerequisites

- .NET Core SDK (version 3.1 or later)
- NATS Server
- NATS.Client NuGet package

## Installation

1. Install the .NET Core SDK from the [official website](https://dotnet.microsoft.com/download).

2. Install the NATS.Client package:
    ```bash
    dotnet add package NATS.Client
    ```

3. Ensure you have a running NATS server. You can download and run the NATS server from [nats.io](https://nats.io/download/).

## Setting Up the NATS Launcher application

Create a new .NET console application for the application lancher:

```bash
dotnet new console -n NATS_Example
cd NATS_Example
dotnet add package NATS.Client
```

Create a Program.cs file with the following code:
```bash
using System.Diagnostics;

class Program
{
    static void Main(string[] args)
    {
        while (true)
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
        }
    }

    static void StartConsumer(string consumerName)
    {
        Console.WriteLine(AppDomain.CurrentDomain.BaseDirectory);
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"run --project ../../../../Consumer/Consumer.csproj {consumerName}", // Adjust the path as needed
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
using NATS.Client;
using System.Text;

namespace Consumer
{
    public class Consumer
    {
        public static void Consume(string[] args)
        {
            Console.WriteLine($"Hello, I am Consumer");
            Options opts = ConnectionFactory.GetDefaultOptions();
            opts.Url = "nats://localhost:4222";

            using (IConnection c = new ConnectionFactory().CreateConnection(opts))
            {
                string subject = "example";
                EventHandler<MsgHandlerEventArgs> msgHandler = (sender, args) =>
                {
                    string receivedMessage = Encoding.UTF8.GetString(args.Message.Data);
                    Console.WriteLine($"{subject}: {receivedMessage}");
                };

                c.SubscribeAsync(subject, msgHandler);
                Console.WriteLine($"Subscribed to subject: {subject}");

                // Keep the application running to listen for messages
                Console.ReadLine();
            }
        }
    }
}
```
Also create a Program.cs file with the following code:
```bash
Consumer.Consumer.Consume(args);
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
using System.Text;

namespace Publisher
{
    public class Publisher
    {
        public static void Publish(string[] args)
        {
            Options opts = ConnectionFactory.GetDefaultOptions();
            opts.Url = "nats://localhost:4222";
            string subject = "example";
            while (true)
            {
                string msg = Console.ReadLine();
                if (msg == null)
                {
                    Console.WriteLine($"{subject}: No message received");
                    Thread.Sleep(1000);
                }
                else
                {
                    using (IConnection c = new ConnectionFactory().CreateConnection(opts))
                    {
                        c.Publish(subject, Encoding.UTF8.GetBytes(msg));

                    }
                }
            }

        }
    }
}


```
Also create a Program.cs file with the following code:
```bash
Publisher.Publisher.Publish(args);
```
Run the publisher:
```bash
dotnet run
```
>**Please configure the solution startup as multiple startup project and change none to start for the project NATS_Example and Publisher application**
# Conclusion
In this tutorial, you have set up a basic NATS publisher and a dynamic consumer in C#. The publisher sends a message to a subject, and the dynamic consumer receives and acknowledges the message. You can expand this basic setup to fit more complex scenarios and business requirements.

For more information on NATS , refer to the [official documentation](https://docs.nats.io/).
