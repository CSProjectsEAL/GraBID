
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using Shared;
using System;
using System.Runtime.Loader;
using System.Text;
using System.Threading;

namespace Logger
{
    class Program
    {
        private static IConnection conn;

        private static IModel channel;
        private static readonly AutoResetEvent WaitHandle = new AutoResetEvent(false);

        static void Main(string[] args)
        {
            AssemblyLoadContext.Default.Unloading += _ => Exit();
            Console.CancelKeyPress += (_, __) => Exit();

            Log.Logger = new LoggerConfiguration()
                .Enrich.WithProperty("name", typeof(Program).Assembly.GetName().Name)
                .WriteTo.Console()
                .CreateLogger();

            Log.Information("Starting...");

            var factory = new ConnectionFactory() { HostName = "rabbit.docker" };
            conn = factory.CreateConnection();
            channel = conn.CreateModel();

            var exchange = "grabid_exchange";
            var routingKey = "mono.data.refined";

            channel.ExchangeDeclare(exchange: exchange, type: "topic");

            var queueName = channel.QueueDeclare().QueueName;
            channel.QueueBind(queue: queueName,
                                  exchange: exchange,
                                  routingKey: routingKey);


            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                var envelope = JsonConvert.DeserializeObject<Envelope<string[]>>(message);
                var key = ea.RoutingKey;
                Console.WriteLine($" [x] Received '{envelope.Id}':'{message }'");
                foreach (var msg in envelope.Payload)
                {
                    Console.WriteLine($" [x] Received :'{msg}'");
                }
            };

            channel.BasicConsume(queue: queueName,
                                 autoAck: true,
                                 consumer: consumer);


            Log.Information("Started");

            WaitHandle.WaitOne();
        }

        private static void Exit()
        {
            Log.Information("Exiting...");
            channel.Close();
            conn.Close();


        }
    }

      
}
