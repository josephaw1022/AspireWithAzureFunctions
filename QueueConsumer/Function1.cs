using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace QueueConsumer
{
    public class Function1
    {
        private readonly ILogger _logger;

        private readonly IConfiguration _configuration;

        public Function1(ILoggerFactory loggerFactory, IConfiguration config)
        {
            _logger = loggerFactory.CreateLogger<Function1>();
            _configuration = config;
        }


        [Function("Function1Consumer")]
        public void Run([RabbitMQTrigger("task_queue", ConnectionStringSetting = "queue")] string myQueueItem)
        {
            _logger.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
        }
    }
}
