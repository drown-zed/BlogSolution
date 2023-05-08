using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Blog.Models;
using Blog.Services;
using Blog.Repositories;

namespace Blog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogPostsController : ControllerBase
    {

        private readonly BlogPostRepository _blogPostRepository;
        private readonly IJwtToken _jwtToken;
        private readonly PostEventProducer _producer;

        public BlogPostsController(BlogPostRepository blogPostRepository, IJwtToken jwtToken, PostEventProducer producer)
        {
            _blogPostRepository = blogPostRepository;
            _jwtToken = jwtToken;
            _producer = producer;
        }

        // GET: api/BlogPosts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BlogPost>>> GetBlogPosts()
        {
            return await _blogPostRepository.GetAllWithUsers();
        }

        // GET: api/BlogPosts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BlogPost>> GetBlogPost(int id)
        {
            var blogPost = await _blogPostRepository.GetByIdWithUser(id);

            if (blogPost == null)
            {
                return NotFound();
            }

            return blogPost;
        }

        // PUT: api/BlogPosts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBlogPost([FromHeader(Name = "Auth-Token")] string authToken, int id, BlogPost blogPost)
        {
            if (_jwtToken.ValidateToken(authToken) == false)
            {
                return BadRequest("Permission dennied");
            }

            if (id != blogPost.Id)
            {
                return BadRequest();
            }
            blogPost.UserId = _jwtToken.GetUserIdFromToken(authToken);

            try
            {
                await _blogPostRepository.Update(blogPost);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_blogPostRepository.Exists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/BlogPosts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<BlogPost>> PostBlogPost([FromHeader(Name = "Auth-Token")] string authToken, BlogPost blogPost)
        {
            if (_jwtToken.ValidateToken(authToken) == false)
            {
                return BadRequest("Permission dennied");
            }
            blogPost.UserId = _jwtToken.GetUserIdFromToken(authToken);

            await _blogPostRepository.CreateAsync(blogPost);

            _producer.sendPostCreated($"https://localhost:44338/api/blog-posts/{blogPost.Id}");

            return CreatedAtAction("GetBlogPost", new { id = blogPost.Id }, blogPost);
        }

        // DELETE: api/BlogPosts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBlogPost([FromHeader(Name = "Auth-Token")] string authToken, int id)
        {
            if (_jwtToken.ValidateToken(authToken) == false)
            {
                return BadRequest("Permission dennied");
            }

            var blogPost = await _blogPostRepository.GetById(id);

            if (blogPost == null)
            {
                return NotFound();
            }

            await _blogPostRepository.DeleteAsync(blogPost);

            return NoContent();
        }
    }
}
