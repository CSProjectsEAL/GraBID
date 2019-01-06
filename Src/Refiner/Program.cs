
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Loader;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Refiner
{
    class Program
    {
        private static readonly AutoResetEvent WaitHandle = new AutoResetEvent(false);

        static void Main(string[] args)
        {
            //To make sure the console application doesn't exit and shutdown the container. And it needs to keep listening on ports.
            AssemblyLoadContext.Default.Unloading += _ => Exit();
            Console.CancelKeyPress += (_, __) => Exit();

            // TODO: Instantiation should be refatored to environment config files
            IConnectionFactory factory = new ConnectionFactory() { HostName = "localhost", UserName = "guest", Password = "guest" };
            string exchangeName = "mono.data.received";

            RefinerService refiner = new RefinerService(factory, exchangeName);

            refiner.Start();
            
            Log.Information("Refiner Started..");

            WaitHandle.WaitOne();
        }

        private static void Exit()
        {
            Log.Information("Exiting Refiner Service...");
        }

        private static void StartupApplication()
        {
            Log.Information("Starting Refiner Service...");
            Log.Logger = new LoggerConfiguration()
            .Enrich.WithProperty("name", typeof(Program).Assembly.GetName().Name)
            .WriteTo.Console()
            .CreateLogger();
        }
    }
}
