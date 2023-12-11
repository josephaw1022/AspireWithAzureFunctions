using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace QueueConsumer
{
    public class Function1
    {
        [Function("Function1Consumer")]
        public void Run([RabbitMQTrigger("taskqueue", ConnectionStringSetting = "queue")] string? myQueueItem)
        {
            Console.WriteLine($"C# Queue trigger function processed: {myQueueItem}");
        }
    }
}
