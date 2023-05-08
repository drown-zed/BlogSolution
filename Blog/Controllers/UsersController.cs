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
    public class UsersController : ControllerBase
    {
        private readonly UserRepository _userRepository;
        private readonly IJwtToken _jwtToken;

        public UsersController(UserRepository userRepository, IJwtToken jwtToken)
        {
            _userRepository = userRepository;
            _jwtToken = jwtToken;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _userRepository.GetAll();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _userRepository.GetById(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser([FromHeader(Name = "Auth-Token")] string authToken, int id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            try
            {
                await _userRepository.Update(user);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_userRepository.Exists(id))
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

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser([FromHeader(Name = "Auth-Token")] string authToken, int id)
        {
            var user = await _userRepository.GetById(id);
            if (user == null)
            {
                return NotFound();
            }

            await _userRepository.DeleteAsync(user);

            return NoContent();
        }
    }
}
