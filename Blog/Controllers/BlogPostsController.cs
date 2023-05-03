using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Blog.Models;
using Blog.Services;

namespace Blog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogPostsController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IJwtToken _jwtToken;
        private readonly PostEventProducer _producer;

        public BlogPostsController(DatabaseContext context, IJwtToken jwtToken, PostEventProducer producer)
        {
            _context = context;
            _jwtToken = jwtToken;
            _producer = producer;
        }

        // GET: api/BlogPosts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BlogPost>>> GetBlogPosts()
        {   
            return await _context.BlogPosts.Include(post => post.User).ToListAsync();
        }

        // GET: api/BlogPosts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BlogPost>> GetBlogPost(int id)
        {
            var blogPost = await _context.BlogPosts.Include(post => post.User).Where(post => post.Id == id).FirstAsync();

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

            _context.Entry(blogPost).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BlogPostExists(id))
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

            _context.BlogPosts.Add(blogPost);
            _context.SaveChanges();

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

            var blogPost = await _context.BlogPosts.FindAsync(id);
            if (blogPost == null)
            {
                return NotFound();
            }

            _context.BlogPosts.Remove(blogPost);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BlogPostExists(int id)
        {
            return _context.BlogPosts.Any(e => e.Id == id);
        }
    }
}
