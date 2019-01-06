using Newtonsoft.Json;
using RabbitMQ.Client;
using Shared;
using System;

namespace Ingestion
{
    class Program
    {
        static void Main(string[] args)
        {
            //Read about Rabbitmq - https://www.rabbitmq.com/tutorials/tutorial-five-dotnet.html
            // for more advanced Message Bus setup - http://masstransit-project.com/MassTransit/ which integrates with RabbitMQ as well
            Console.WriteLine($"Starting Ingestor");
            var factory = new ConnectionFactory() { HostName = "rabbit.docker" };           

            using (IConnection conn = factory.CreateConnection())
            {
                using (IModel channel = conn.CreateModel())
                {
                    var exchange = "grabid_exchange";
                    //This would properly be a http call to get new data from Mono
                    var message = "{'message':'Hello ','users':'Alexander, Bogdan, Elitsa, David'}";
                    var routingKey = "mono.data.received";

                    channel.ExchangeDeclare(exchange: exchange, type: "topic");
                    var envelope = new Envelope<string>(Guid.NewGuid(), message);
                    var envelopedMessage = JsonConvert.SerializeObject(envelope);
                    byte[] messageBodyBytes = System.Text.Encoding.UTF8.GetBytes(envelopedMessage);
                   
                    channel.BasicPublish(exchange,routingKey , null, messageBodyBytes);
                    Console.WriteLine(" [x] Sent '{0}':'{1}'", routingKey, message);
                }
            }
            Console.WriteLine($"Stopping Ingestor");
        }
    }
}
