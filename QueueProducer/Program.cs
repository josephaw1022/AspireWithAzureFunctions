using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MassTransit;
using Microsoft.Extensions.Configuration;
using SharedClassLibraryForConsumerAndProducer;
using MassTransit.Transports.Fabric;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((hostContext, services) =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        services.AddMassTransit(config =>
        {
            config.SetKebabCaseEndpointNameFormatter();

            var rabbitMQConnectionString = hostContext.Configuration.GetConnectionString("queue");

            config.UsingRabbitMq((context, cfg) =>
            {

                cfg.Host(rabbitMQConnectionString);

                // Configure an exchange for ArbitraryMessage and bind it to the queue
                cfg.Message<ArbitraryMessage>(m => m.SetEntityName("TaskQueue"));
                cfg.Publish<ArbitraryMessage>(p => p.ExchangeType = ExchangeType.Direct.ToString().ToLower());
            });
        });

        services.AddMassTransitHostedService();
    })
    .Build();

host.Run();
