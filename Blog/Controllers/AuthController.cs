using Blog.DTO.Input;
using Blog.Models;
using Blog.Repositories;
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
        private IJwtToken _jwtToken;
        private UserRepository _userRepository;

        public AuthController(IConfiguration configuration, UserRepository userRepository, IJwtToken jwtToken)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _jwtToken = jwtToken;
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(User user, CancellationToken cancellationToken)
        {
            var hash = CreatePasswordHash(user.Password);
            user.Password = hash;
            await _userRepository.CreateAsync(user);

            return Ok();
        }


        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(LoginParameters request)
        {
            var hash = CreatePasswordHash(request.Password);


            User? user = await _userRepository.FindSingleByNicknameAsync(request.Username);

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
