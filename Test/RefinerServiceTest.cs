using NUnit.Framework;
using Refiner;
using System.Collections.Generic;
using RabbitMQ.Client;
using Serilog;

namespace Tests
{
    public class RefinerServiceTest
    {
        private RefinerService refiner;
        
        [SetUp]
        public void Setup()
        {
            refiner = new RefinerService(new LoggerConfiguration().WriteTo.Console().CreateLogger());
        }

        [Test]
        public void TestCanPullMessagesFromQueue()
        {
            refiner.RetrieveMessageFromQueue("mono.data.received");
            using (IConnection conn = new ConnectionFactory() {HostName = "localhost", UserName = "guest", Password = "guest"}.CreateConnection())
            {
                using (IModel channel = conn.CreateModel())
                {
                    BasicGetResult result = channel.BasicGet("mono.data.refined", false);
                    Assert.IsNotNull(result.Body);
                }
            }
        }

        [Test]
        public void TestCanPullMessagesFromDifferentQueues()
        {
        }

        [Test]
        public void TestCanStoreMessagePayloadInMongoDB()
        {
        }
    }
}