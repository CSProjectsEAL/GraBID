using System;
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

        public RefinerService(IConnectionFactory factory, string exchangeName, IList<DataSource> dataSources)
        {
            _exchange = exchangeName;
            try
            {
                _conn = factory.CreateConnection();
            }
            catch (RabbitMQ.Client.Exceptions.BrokerUnreachableException e)
            {
                Log.Error("Couldn't establish connection to RabbitMQ, Msg: " + e.Message + ", Trace:" + e.StackTrace);
            }
            _channel = _conn.CreateModel();
            _dataSources = dataSources;
        }

        public void Start()
        {
            Log.Information($"Adding listener on RabbitMQ {_exchange}:{_channel}");
            StartListening();
        }

        private DataSource GetDataSource(string key)
        {
            DataSource source = null;
            foreach(DataSource s in _dataSources)
            {
                if (s.KeyOrigin == key) source = s;
            }
            return source;
        }

        public void Stop()
        {
            _channel.Close();
            _conn.Close();
        }

        private void StartListening()
        {
            var queueName = _channel.QueueDeclare().QueueName;

            _channel.QueueDeclare(queueName, true, false, false, null);

            _channel.ExchangeDeclare(_exchange, "fanout");

            _channel.QueueBind(queueName, _exchange, "");

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                var key = ea.RoutingKey;
                Log.Information($" [x] Received '{key}':'{message}'");
                var envelope = JsonConvert.DeserializeObject<Envelope<string>>(message);

                DataSource source = GetDataSource(key);
                if (source != null)
                {
                    foreach(DataHandler handler in source.DataHandlers)
                    {
                        string processedData = handler.Handle(envelope.Payload);
                        ForwardMessageToQueue(handler.Destination, processedData);
                    }
                }
                else
                {
                    Log.Error("Source is not configured.");
                }
            };
            _channel.BasicConsume(queueName, autoAck: true, consumer: consumer);
        }

        private void ForwardMessageToQueue(string key, dynamic cleanData)
        {
            var message = JsonConvert.SerializeObject(cleanData);
            byte[] messageBodyBytes = Encoding.UTF8.GetBytes(message);
            
            using (_conn)
            {
                using (_channel)
                {
                    _channel.QueueDeclare(key, true, false, false, null);
                    
                    _channel.ExchangeDeclare(_exchange, "fanout");
                    
                    _channel.QueueBind(key, _exchange, "");

                    _channel.BasicPublish(key, "", _channel.CreateBasicProperties(), messageBodyBytes);
                    Log.Information(" [x] Sent '{0}':'{1}'", _exchange, message);
                }
            }
        }
    }
}