using Blog.Models;
using Microsoft.EntityFrameworkCore;

namespace Blog.Repositories
{
    public class CommentRepository : IDisposable
    {
        private bool _disposed = false;
        private DatabaseContext _context;

        public CommentRepository(DatabaseContext databaseContext)
        {
            _context = databaseContext;
        }

        public async Task CreateAsync(Comment comment)
        {
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
        }

        public void Create(Comment comment)
        {
            _context.Comments.Add(comment);
            _context.SaveChanges();
        }

        public async Task<List<Comment>> GetAll()
        {
            return await _context.Comments.ToListAsync();
        }

        public async Task<Comment?> GetById(int id)
        {
            return await _context.Comments.FindAsync(id);
        }

        public async Task DeleteAsync(Comment comment)
        {
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Comment comment)
        {
            _context.Entry(comment).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public bool Exists(int id)
        {
            return _context.Comments.Any(e => e.Id == id);
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
