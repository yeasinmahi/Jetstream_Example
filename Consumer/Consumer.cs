using NATS.Client.JetStream;
using NATS.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
