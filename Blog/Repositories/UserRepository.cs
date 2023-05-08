using Blog.Models;
using Microsoft.EntityFrameworkCore;

namespace Blog.Repositories
{
    public class UserRepository : IDisposable
    {
        private bool _disposed = false;
        private DatabaseContext _context;

        public UserRepository(DatabaseContext databaseContext)
        {
            _context = databaseContext;
        }

        public async Task CreateAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User?> FindSingleByNicknameAsync(string nickname)
        {
            return await _context.Users.SingleOrDefaultAsync(x => x.Nickname == nickname);
        }

        public async Task<List<User>> GetAll()
        {
            return await _context.Users.ToListAsync();
        }
        public async Task<User?> GetById(int id)
        {
            return await _context.Users.FindAsync(id);
        }


        public async Task DeleteAsync(User comment)
        {
            _context.Users.Remove(comment);
            await _context.SaveChangesAsync();
        }

        public async Task Update(User user)
        {
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public bool Exists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
