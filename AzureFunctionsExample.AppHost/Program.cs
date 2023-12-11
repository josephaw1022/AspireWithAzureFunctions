var builder = DistributedApplication.CreateBuilder(args);

// var cache = builder.AddRedisContainer("cache");

// var apiservice = builder.AddProject<Projects.AzureFunctionsExample_ApiService>("apiservice");


var queue = builder.AddRabbitMQContainer("queue")
                   .WithVolumeMount("VolumeQueue", "/var/lib/rabbitmq", VolumeMountType.Named);

var azureFunctionQueueProducer = builder.AddProject<Projects.QueueProducer>("queueproducer")
    .WithReference(queue);

//var azureFunctionQueueConsumer = builder.AddProject<Projects.QueueConsumer>("queueconsumer")
//    .WithReference(queue);


var massTransitConsumer = builder.AddProject<Projects.MTConsumer>("masstransitconsumer")
        .WithReference(queue);

//var frontendApp = builder.AddProject<Projects.AzureFunctionsExample_Web>("webfrontend")
//    .WithReference(cache)
//    .WithReference(apiservice)
//    .WithReference(azureFunctionQueueProducer);

await builder.Build().RunAsync();
