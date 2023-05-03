namespace Blog.Services
{
    using Confluent.Kafka;
    using System.Security.Policy;

    public class PostEventProducer
    {
        private const string POST_TOPIC = "post";

        private ProducerConfig _config;
        public PostEventProducer(IConfiguration configuration)
        {
            _config = new ProducerConfig
            {
                BootstrapServers = configuration.GetValue<string>("ProducerHost")
            };
        }

        public void sendPostCreated(string url)
        {
            using (var producer = new ProducerBuilder<Null, string>(_config).Build()) 
            {
                producer.Produce(POST_TOPIC, new Message<Null, string> { Value = $"A new blog post has been published! Check it out <a href=\"{url}\"> here</a>" });
            }
            
        }
    }
}
