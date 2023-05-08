using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Blog.Models;
using Blog.Services;
using Blog.Repositories;

namespace Blog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private CommentRepository _commentRepository;
        private readonly IJwtToken _jwtToken;

        public CommentsController(CommentRepository commentRepository, IJwtToken jwtToken)
        {
            _commentRepository = commentRepository;
            _jwtToken = jwtToken;
        }

        // GET: api/Comments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Comment>>> GetComments()
        {
            return await _commentRepository.GetAll();
        }

        // GET: api/Comments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Comment>> GetComment(int id)
        {
            var comment = await _commentRepository.GetById(id);

            if (comment == null)
            {
                return NotFound();
            }

            return comment;
        }

        // PUT: api/Comments/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutComment([FromHeader(Name = "Auth-Token")] string authToken, int id, Comment comment)
        {
            if (_jwtToken.ValidateToken(authToken) == false)
            {
                return BadRequest("Permission dennied");
            }

            if (id != comment.Id)
            {
                return BadRequest();
            }

            comment.UserId = _jwtToken.GetUserIdFromToken(authToken);


            try
            {
                await _commentRepository.Update(comment);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_commentRepository.Exists(id))
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

        // POST: api/Comments
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Comment>> PostComment([FromHeader(Name = "Auth-Token")] string authToken, Comment comment)
        {
            if (_jwtToken.ValidateToken(authToken) == false)
            {
                return BadRequest("Permission dennied");
            }

            comment.UserId = _jwtToken.GetUserIdFromToken(authToken);

            await _commentRepository.CreateAsync(comment);

            return CreatedAtAction("GetComment", new { id = comment.Id }, comment);
        }

        // DELETE: api/Comments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment([FromHeader(Name = "Auth-Token")] string authToken, int id)
        {
            if (_jwtToken.ValidateToken(authToken) == false)
            {
                return BadRequest("Permission dennied");
            }

            var comment = await _commentRepository.GetById(id);
            if (comment == null)
            {
                return NotFound();
            }

            await _commentRepository.DeleteAsync(comment);

            return NoContent();
        }
    }
}
