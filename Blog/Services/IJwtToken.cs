using Blog.Models;

namespace Blog.Services
{
    public interface IJwtToken
    {
        string CreateToken(User user);
        bool ValidateToken(string token);
        int GetUserIdFromToken(string token);
    }
}