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

