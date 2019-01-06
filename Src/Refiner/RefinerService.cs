using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using Shared;

namespace Refiner
{
    public class RefinerService
    {
        private static IConnection _conn;
        private static IModel _channel;
        private static string _exchange;
        private IList<DataSource> _dataSources;

        public RefinerService(IConnectionFactory factory, IModel _channel, string exchangeName, IList<DataSource> dataSources)
        {
            _exchange = exchangeName;
            _conn = factory.CreateConnection();
            _channel = _conn.CreateModel();
            _dataSources = dataSources;
        }


        public void Start()
        {
            AddListeners();
        }

        private void AddListeners()
        {
            Log.Information("Adding listeners on RabbitMQ channels.");
            foreach (DataSource s in _dataSources)
            {
                StartListening(s);
                Log.Information($"Listener on Exchange:{s.Exchange}, Channel: {s.Queue} successfully started.");
            }
        }

        private void StartListening(DataSource datasource)
        {
            using (_conn)
            {
                using (_channel)
                {
                    _channel.QueueDeclare(datasource.Queue, true, false, false, null);

                    _channel.ExchangeDeclare(_exchange, "fanout");

                    _channel.QueueBind(datasource.Queue, _exchange, "");

                    var consumer = new EventingBasicConsumer(_channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        var key = ea.RoutingKey;
                        Log.Information($" [x] Received '{key}':'{message}'");
                        var envelope = JsonConvert.DeserializeObject<Envelope<string>>(message);

                        foreach (DataHandler d in datasource.DataHandlers)
                        {
                            string processedData = d.Handle(envelope.Payload);

                            ForwardMessageToQueue("mono.processed.data", processedData);
                        }
                    };
                    _channel.BasicConsume(datasource.Queue, true, consumer);
                }
            }
        }

        private void ForwardMessageToQueue(string queue, JToken cleanData)
        {
            var message = JsonConvert.SerializeObject(cleanData);
            byte[] messageBodyBytes = Encoding.UTF8.GetBytes(message);
            
            using (_conn)
            {
                using (_channel)
                {
                    _channel.QueueDeclare(queue, true, false, false, null);
                    
                    _channel.ExchangeDeclare(_exchange, "fanout");
                    
                    _channel.QueueBind(queue, _exchange, "");

                    _channel.BasicPublish(queue, "", _channel.CreateBasicProperties(), messageBodyBytes);
                    Log.Information(" [x] Sent '{0}':'{1}'", _exchange, message);
                }
            }
        }
    }
}