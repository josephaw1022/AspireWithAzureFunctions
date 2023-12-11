using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MassTransit;
using System;
using System.Reflection;
using System.Threading.Tasks;
using SharedClassLibraryForConsumerAndProducer;

namespace MTConsumer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).Build().RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddMassTransit(config =>
                    {
                        var entryAssembly = Assembly.GetEntryAssembly();
                        config.AddConsumers(entryAssembly);
                        config.AddSagaStateMachines(entryAssembly);
                        config.AddSagas(entryAssembly);
                        config.AddActivities(entryAssembly);

                        // Register the TaskQueueConsumer
                        config.AddConsumer<TaskQueueConsumer>();

                        // Retrieve RabbitMQ connection string from configuration
                        var rabbitMQConnectionString = hostContext.Configuration.GetConnectionString("queue");

                        // Configure RabbitMQ as the transport
                        config.UsingRabbitMq((context, cfg) =>
                        {
                            cfg.Host(new Uri(rabbitMQConnectionString));

                            // Register the endpoint for 'taskqueue'
                            cfg.ReceiveEndpoint("TaskQueue", e =>
                            {
                                e.ConfigureConsumer<TaskQueueConsumer>(context);
                            });

                            cfg.ConfigureEndpoints(context);
                        });
                    });
                });
    }
}
