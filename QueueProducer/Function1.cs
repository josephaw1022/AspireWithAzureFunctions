using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SharedClassLibraryForConsumerAndProducer;
using MassTransit;

namespace QueueProducer
{
    public class Function1
    {
        private readonly ILogger _logger;
        private readonly IBus _bus;

        public Function1(ILoggerFactory loggerFactory, IBus bus)
        {
            _logger = loggerFactory.CreateLogger<Function1>();
            _bus = bus;
        }

        [Function("Function1")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var endpoint = await _bus.GetSendEndpoint(new Uri("queue:TaskQueue"));

            var rabbitMqMessage = new ArbitraryMessage
            {
                Name = "Azure function",
                Message = $"Azure function {nameof(Function1)} was called. Call {Guid.NewGuid()}"
            };

            // Send the message using the bus
            await endpoint.Send(rabbitMqMessage);

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json");
            response.WriteString("Message pushed to queue");

            return response;
        }
    }
}
