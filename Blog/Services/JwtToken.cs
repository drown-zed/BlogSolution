using Blog.Models;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Blog.Services
{
    public class JwtToken : IJwtToken
    {
        private IConfiguration _configuration;

        public JwtToken(IConfiguration config)
        {
            _configuration = config;
        }
        public string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Nickname),
                new Claim(ClaimTypes.Actor, user.Id.ToString())
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("Jwt:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
        public bool ValidateToken(string token)
        {
            if (token == null)
                return false;

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                    _configuration.GetSection("Jwt:Token").Value));
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var username = jwtToken.Claims.First(x => x.Type == ClaimTypes.Name).Value;

                return true;
            }
            catch
            {
                return false;
            }
        }
        public int GetUserIdFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadToken(token);
            var jwtSecurityToken= jwtToken as JwtSecurityToken;

            if (jwtSecurityToken == null)
            {
                throw new Exception("Could not read token");
            }

            var userId = jwtSecurityToken.Claims.First(claim => claim.Type == ClaimTypes.Actor).Value;
            return System.Int32.Parse(userId);
        }
    }
}
