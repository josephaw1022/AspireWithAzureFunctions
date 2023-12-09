using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace QueueProducer
{
    public class Function1
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        public Function1(ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            _logger = loggerFactory.CreateLogger<Function1>();
            _configuration = configuration;
        }

        [Function("Function1")]
        public  HttpResponseData Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");



            var configDictionary = _configuration.AsEnumerable()
                                    .Where(c => c.Value != null)
                                    .ToDictionary(c => c.Key, c => c.Value);

            
            var serializedConfiguration = JsonConvert.SerializeObject(configDictionary);


            PublishRabbitMqMessageAsync($"Azure function {nameof(Function1)} was called. Call {Guid.NewGuid()}");

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json");

            response.WriteString("Message pushed to queue");

            return response;
        }
    

        private void PublishRabbitMqMessageAsync(string message)
        {

            var connectionString = _configuration["ConnectionStrings:queue"];
            var factory = new ConnectionFactory() { Uri = new Uri(connectionString) };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "task_queue",
                     durable: true,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);

                ReadOnlyMemory<byte> body = System.Text.Encoding.UTF8.GetBytes(message);


                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;

                channel.BasicPublish(exchange: string.Empty,
                     routingKey: "task_queue",
                     basicProperties: properties,
                     body: body);
            }
        }


    }



}
