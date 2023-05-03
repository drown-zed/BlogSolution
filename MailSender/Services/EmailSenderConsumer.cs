using Mailjet.Client.TransactionalEmails;
using Mailjet.Client;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Serilog;
using Microsoft.Extensions.Configuration;

namespace MailSender.Services
{
    internal class EmailSenderConsumer
    {
        private readonly ILogger<EmailSenderConsumer> _logger;
        private readonly IConfiguration _config;
        public EmailSenderConsumer(ILogger<EmailSenderConsumer> logger, IConfiguration configuration)
        {
            _logger = logger;
            _config = configuration;
        }

        public void Execute(CancellationToken cancellationToken)
        {
            MailjetClient client = new(
                _config.GetValue<string>("Mailjet:Key"),
                _config.GetValue<string>("Mailjet:Secret")
            );

            var config = new ConsumerConfig
            {
                BootstrapServers = _config.GetValue<string>("Post:ConsumerHost"),
                GroupId = "posts",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            consumer.Subscribe("post");
            Log.Logger.Information("Consumer started");
            while (cancellationToken.IsCancellationRequested == false)
            {
                _logger.LogInformation("Consuming...");
                try
                {
                    var consumeResult = consumer.Consume(cancellationToken);

                    // handle consumed message.
                    var message = consumeResult.Message.Value;

                    // construct your email with builder
                    var email = new TransactionalEmailBuilder()
                        .WithFrom(new SendContact(_config.GetValue<string>("Post:FromEmail")))
                        .WithSubject("New blog post created")
                        .WithHtmlPart("<h1>New blog post was created</h1> <br>" + message)
                        .WithTo(new SendContact(_config.GetValue<string>("Post:NotificationEmail")))
                        .Build();

                    _logger.LogInformation($"Sending message \"{message}\"");

                    // invoke API to send email
                    client.SendTransactionalEmailAsync(email).Wait(cancellationToken);

                    _logger.LogInformation("Sent");
                }
                catch(Exception ex)
                {
                    _logger.LogError($"Closing the consumer, {ex.Message}");
                }

            }
            consumer.Close();
        }
    }
}
