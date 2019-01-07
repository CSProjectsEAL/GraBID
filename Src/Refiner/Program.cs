
using RabbitMQ.Client;
using Serilog;
using System;
using System.Collections.Generic;
using System.Runtime.Loader;
using System.Threading;

namespace Refiner
{
    class Program
    {
        private static readonly AutoResetEvent WaitHandle = new AutoResetEvent(false);
        private static RefinerService _refiner;
        static void Main(string[] args)
        {
            StartupApplication();
            WaitHandle.WaitOne();
        }

        private static void Exit()
        {
            Log.Information("Exiting refiner service gracefully...");
            _refiner.Stop();
        }

        private static void StartupApplication()
        {
            Log.Information("Starting refiner service...");
            Log.Logger = new LoggerConfiguration()
            .Enrich.WithProperty("name", typeof(Program).Assembly.GetName().Name)
            .WriteTo.Console()
            .CreateLogger();

            //To make sure the console application doesn't exit and shutdown the container. And it needs to keep listening on ports.
            AssemblyLoadContext.Default.Unloading += _ => Exit();
            Console.CancelKeyPress += (_, __) => Exit();

            // Instantiation should be refatored to environment config files
            IConnectionFactory factory = new ConnectionFactory() { HostName = "rabbit.docker" };
            string exchangeName = "grabid_exchange";
            List<DataSource> dataSources = LoadConfigurations();

            RefinerService _refiner = new RefinerService(factory, exchangeName, dataSources);

            _refiner.Start();
            Log.Information("Refiner Started..");
        }

        private static List<DataSource> LoadConfigurations()
        {
            List<DataSource> dataSources = new List<DataSource>();

            //Configurations for the one source
            string keyOrigin = "mono.data.ingested";
            string destination = "mono.data.refined";
            List<DataHandler> dataHandlers = new List<DataHandler>();

            IDBFacade db = new MongoDBConnector("mongodb://localhost:27017");

            DataHandler handler = new DataHandler(
                new TransactionDataProcessor(),
                destination,
                db
                );
            dataHandlers.Add(handler);
            DataSource ds = new DataSource() {
                DataHandlers = dataHandlers, KeyOrigin=keyOrigin
            };
            dataSources.Add(ds);
            return dataSources; 
        }
    }
}
