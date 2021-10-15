using MassTransit;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TicketSales.Core.Application;

namespace TicketSales.Core.Ticketsm
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false);

            IConfiguration config = builder.Build();

            var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host(new Uri(config["MassTransit:RabbitMqUri"]), hostConfigurator =>
                {
                    hostConfigurator.Username(config["MassTransit:RabbitMqUser"]);
                    hostConfigurator.Password(config["MassTransit:RabbitMqPassword"]);
                });

                cfg.ReceiveEndpoint("tickets", e =>
                {
                    e.PrefetchCount = 16;

                    e.Consumer<TestCommandHandler>();
                });
            });

            var source = new CancellationTokenSource(TimeSpan.FromSeconds(10));

            await busControl.StartAsync(source.Token);
            try
            {
                Console.WriteLine($"Application started {System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}");
                Console.WriteLine("Press enter to exit");

                await Task.Run(() => Console.ReadLine());
            }
            finally
            {
                await busControl.StopAsync();
            }
        }
    }
}
