using System.Text;
using System.Text.Json;
using Blog.Models;
using Hangfire;
using Blog.DTO.Output;
using Blog.DTO.Input;
using Blog.Repositories;

namespace Blog.Services
{
    public class AIBlogPostGenerator : IAIBlogPostGenerator
    {
        private static BlogPostRepository? _blogPostRepository;
        private IBackgroundJobClient _client;
        private IConfiguration _configuration;
        private PostEventProducer _producer;
        public AIBlogPostGenerator(BlogPostRepository blogPostRepository, IBackgroundJobClient client, IConfiguration configuration, PostEventProducer producer)
        {
            _blogPostRepository = blogPostRepository;
            _client = client;
            _configuration = configuration;
            _producer = producer;
        }

        public void GeneratePostAutomatically(string title , int userId)
        {
            _client.Enqueue(() => GeneratePost(title, userId));
        }

        public async Task GeneratePost(string title, int userId)
        {
            HttpClient client = new HttpClient();
            StringContent content = createRequestContent(title);
            string bearerToken = _configuration.GetValue<string>("OpenAIToken");
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {bearerToken}");
            var response = await client.PostAsync("https://api.openai.com/v1/completions", content);
            var stringResponse = await response.Content.ReadAsStringAsync();
            var aiResponse = JsonSerializer.Deserialize<AIOutputParameters>(stringResponse);

            if (aiResponse == null)
            {
                throw new Exception("Could not parse JSON");
            }

            var blogPost = new BlogPost();
            blogPost.Title = title;
            blogPost.Body = aiResponse.Choices[0].Text ?? "";
            blogPost.UserId = userId;
            if (_blogPostRepository == null)
            {
                throw new Exception("Context is null, wrong database configuration?");
            }
            await _blogPostRepository.CreateAsync(blogPost);
            _producer.sendPostCreated($"https://localhost:44338/api/blog-posts/{blogPost.Id}");
            
        }

        private static StringContent createRequestContent(string title)
        {
            var request = new AIInputParameters();
            request.Prompt = title + " in 100 words";
            request.MaxTokens = 500;
            request.Temperature = 0;
            request.Model = "text-davinci-003";

            string jsonString = JsonSerializer.Serialize(request);

            return new StringContent(jsonString, Encoding.UTF8, "application/json");
        }
    }
}
