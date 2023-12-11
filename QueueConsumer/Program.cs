using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Threading;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((context, services) =>
    {
        // Add logging
        services.AddLogging();

        // Retrieve RabbitMQ connection string from configuration
        var configuration = context.Configuration;
        var rabbitMQConnectionString = configuration.GetConnectionString("queue");

        if(string.IsNullOrEmpty(rabbitMQConnectionString))
        {
            throw new InvalidOperationException("Please specify a valid RabbitMQ connection string (named 'queue') in the appSettings.json file, your Azure Functions Settings, or via reference with Aspire.");
        }

        var serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetService<ILogger<Program>>();


        // Try to establish a connection to RabbitMQ with retries
        if (!TryConnectToRabbitMQ(rabbitMQConnectionString, maxAttempts: 5))
        {
            Console.WriteLine("Failed to connect to RabbitMQ. Exiting application.");
            Environment.Exit(1); // Exit the application if RabbitMQ is not ready
        }
    })
    .Build();

host.Run();


// Method to try connecting to RabbitMQ with retries
bool TryConnectToRabbitMQ(string connectionString, int maxAttempts)
{
    for (int attempt = 0; attempt < maxAttempts; attempt++)
    {
        try
        {
            var factory = new ConnectionFactory() { Uri = new Uri(connectionString) };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "taskqueue",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            return true; // Successful connection and queue declaration
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Attempt {attempt + 1} failed: {ex.Message}");
            Thread.Sleep(5000); // Wait 5 seconds before retrying
        }
    }

    return false; // Failed to connect after all attempts
}