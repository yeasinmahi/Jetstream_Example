using NATS.Client;
using NATS.Client.JetStream;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
