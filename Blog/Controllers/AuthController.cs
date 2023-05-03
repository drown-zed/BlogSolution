using Blog.DTO.Input;
using Blog.Models;
using Blog.Services;
using CryptSharp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IConfiguration _configuration;
        private DatabaseContext _context;
        private IJwtToken _jwtToken;

        public AuthController(IConfiguration configuration, DatabaseContext context, IJwtToken jwtToken)
        {
            _configuration = configuration;
            _context = context;
            _jwtToken = jwtToken;
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(User user, CancellationToken cancellationToken)
        {
            var hash = CreatePasswordHash(user.Password);
            user.Password = hash;
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok();
        }


        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(LoginParameters request)
        {
            var hash = CreatePasswordHash(request.Password);


            User? user = await _context.Users.SingleOrDefaultAsync(x => x.Nickname == request.Username);

            if (user == null)
            {
                return BadRequest("User not found");
            }

            if (!VerifyPasswordHash(request.Password, user.Password))
            {
                return BadRequest("Wrong password.");
            }

            var token = _jwtToken.CreateToken(user);

            return Ok(token);
        }

        private string CreatePasswordHash(string password)
        {
            string salt = _configuration.GetValue<string>("PasswordSalt");
            string hash = Crypter.Blowfish.Crypt(password, salt);
            return hash;
        }

        private bool VerifyPasswordHash(string password, string passwordHash)
        {
            return Crypter.CheckPassword(password, passwordHash);
        }
    }
}
