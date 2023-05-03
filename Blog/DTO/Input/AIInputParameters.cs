using System.Text.Json.Serialization;

namespace Blog.DTO.Input
{
    public class AIInputParameters
    {
        [JsonPropertyName("prompt")]
        public string? Prompt { get; set; }

        [JsonPropertyName("max_tokens")]
        public int MaxTokens { get; set; }

        [JsonPropertyName("temperature")]
        public int Temperature { get; set; }
        [JsonPropertyName("model")]
        public string? Model { get; set; }
    }
}
