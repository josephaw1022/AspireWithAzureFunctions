using MassTransit;
using SharedClassLibraryForConsumerAndProducer;

namespace MTConsumer
{
    public class TaskQueueConsumer : IConsumer<ArbitraryMessage>
    {
        public async Task Consume(ConsumeContext<ArbitraryMessage> context)
        {
            Console.WriteLine("TaskQueueConsumer.Consume() called");


            var message = context.Message;

            Console.WriteLine($"Received message: {message.Name} {message.Message}");
            // Handle the message
            // For example: Process the data in the message
        }
    }

}
