using Blog.DTO;
using Blog.Models;
using Blog.Services;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogPostsAiGeneratorController : ControllerBase
    {
        private readonly IAIBlogPostGenerator _generator;
        private readonly IJwtToken _jwtToken;

        public BlogPostsAiGeneratorController(IAIBlogPostGenerator generator, IJwtToken jwtToken)
        {
            _generator = generator;
            _jwtToken = jwtToken;
        }

        // POST: api/BlogPostsAiGenerator
        [HttpPost]
        
        public IActionResult PostBlogPost([FromHeader(Name = "Auth-Token")] string authToken, string title)
        {
            if (_jwtToken.ValidateToken(authToken) == false)
            {
                return BadRequest("Permission dennied");
            }
            int userId = _jwtToken.GetUserIdFromToken(authToken);
            _generator.GeneratePostAutomatically(title, userId);
            return NoContent();
        }
    }
}
