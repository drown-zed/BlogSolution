using Blog.Models;
using Microsoft.EntityFrameworkCore;

namespace Blog.Repositories
{
    public class BlogPostRepository : IDisposable
    {
        private bool _disposed = false;
        private DatabaseContext _context;

        public BlogPostRepository(DatabaseContext databaseContext)
        {
            _context = databaseContext;
        }

        public async Task CreateAsync(BlogPost post)
        {
            _context.BlogPosts.Add(post);
            await _context.SaveChangesAsync();
        }

        public void Create(BlogPost blogPost)
        {
            _context.BlogPosts.Add(blogPost);
            _context.SaveChanges();
        }

        public async Task<List<BlogPost>> GetAllWithUsers()
        {
            return await _context.BlogPosts.Include(post => post.User).ToListAsync();
        }

        public async Task<BlogPost> GetByIdWithUser(int id)
        {
            return await _context.BlogPosts.Include(post => post.User).Where(post => post.Id == id).FirstAsync();
        }

        public async Task<BlogPost?> GetById(int id)
        {
            return await _context.BlogPosts.FindAsync(id);
        }

        public async Task DeleteAsync(BlogPost blogPost)
        {
            _context.BlogPosts.Remove(blogPost);
            await _context.SaveChangesAsync();
        }

        public async Task Update(BlogPost blogPost)
        {
            _context.Entry(blogPost).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public bool Exists(int id)
        {
            return _context.BlogPosts.Any(e => e.Id == id);
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
