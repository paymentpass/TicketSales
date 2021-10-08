using MassTransit;
using System;
using System.Threading;
using System.Threading.Tasks;
using TicketSales.Core.Application;

namespace TicketSales.Core.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host(new Uri("rabbitmq://localhost"), hostConfigurator =>
                {
                    hostConfigurator.Username("");
                    hostConfigurator.Password("");
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
