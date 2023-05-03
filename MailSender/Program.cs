namespace MailSender
{
    using Confluent.Kafka;
    using System.Threading;
    using Microsoft.Extensions.DependencyInjection;
    using MailSender.Services;
    using Microsoft.Extensions.Logging;
    using Serilog;
    using Microsoft.Extensions.Configuration;

    internal class Program
    {
        static void Main()
        {

           var logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();

            var currentDirectory = Directory.GetCurrentDirectory();

            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(currentDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            var serviceProvider = new ServiceCollection()
                .AddSingleton<EmailSenderConsumer>()
                .AddSingleton(configuration)
                .AddLogging(asd => asd.AddSerilog(logger))
                .BuildServiceProvider();

            var service = serviceProvider.GetRequiredService<EmailSenderConsumer>();

            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (s, e) =>
            {
                Log.Information("Canceling...");
                cts.Cancel();
                e.Cancel = true;
            };

            service.Execute(cts.Token);

        }

        static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            Log.Information("Exiting th1 a");
        }
    }
}