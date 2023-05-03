namespace Blog.Services
{
    public interface IAIBlogPostGenerator
    {
        void GeneratePostAutomatically(string title, int userId);
    }
}