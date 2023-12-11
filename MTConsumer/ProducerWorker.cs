using MassTransit;
using SharedClassLibraryForConsumerAndProducer;

namespace MTConsumer;

public class Worker : BackgroundService
{
    private readonly IBus _bus;

    public Worker(IBus bus)
    {
        _bus = bus;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {

            try
            {
                await _bus.Send(new ArbitraryMessage
                {
                    Name = "Worker",
                    Message = $"Worker was called. Call {Guid.NewGuid()}"
                }, stoppingToken);

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
    }
}