using TechTalkBlog.Models;

namespace TechTalkBlog.Services.Interfaces
{
    public interface IBlogService
    {
        public Task<IEnumerable<BlogPost>> GetAllBlogPostsAsync(int? tagId);
        public Task<BlogPost> GetBlogDetailsAsync(int? id);

        public Task CreateBlogPostAsync(BlogPost blogPost, IEnumerable<int> selected);

        public Task<BlogPost> EditBlogPostAsync(BlogPost blogPost, IEnumerable<int> selected);

        public Task<BlogPost> DeleteBlogPostAsync_Get(int? id);

        public Task UpdateBlogPostAsync(BlogPost blogPost);

        public Task<BlogPost> GetBlogByIdAsync(int? id);
    }
}
