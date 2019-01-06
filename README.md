# Example Setup

## Requirements 

* Docker
* Visual Studio (or vscode)

## Test

`docker-compose up --build`

## Console App and Docker
Usually the container will exit when the console app has been executed and `Console.ReadLine()` won't help here, so see below for how to make it work.  

```

using Serilog;
using System;
using System.Runtime.Loader;
using System.Text;
using System.Threading;

namespace YOURNAMESPACE
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

            //ADD YOUR CODE HERE, IF YOU NEED IT TO LISTEN FOR EVENTS ON RABBITMQ

            Log.Information("Started");

            WaitHandle.WaitOne();
        }

        private static void Exit()
        {
            Log.Information("Exiting...");
        }
    }
      
}

```