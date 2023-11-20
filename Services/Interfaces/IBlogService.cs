using TechTalkBlog.Models;

namespace TechTalkBlog.Services.Interfaces
{
    public interface IBlogService
    {
        public Task<IEnumerable<BlogPost>> GetAllBlogPostsAsync();

        public Task<IEnumerable<BlogPost>> GetAllArchivedBlogPostsAsync(int? tagId);

        public Task<BlogPost> GetBlogDetailsAsync(int? id);

        public Task CreateBlogPostAsync(BlogPost blogPost, IEnumerable<int> selected);

        public Task<BlogPost> EditBlogPostAsync(BlogPost blogPost, IEnumerable<int> selected);

        public Task<BlogPost> DeleteBlogPostAsync_Get(int? id);

        public Task UpdateBlogPostAsync(BlogPost blogPost);

        public Task<BlogPost> GetBlogByIdAsync(int? id);

        public Task<IEnumerable<Category>> GetCategoriesAsync();
        
        public Task<IEnumerable<BlogPost>> GetPopularBlogs();

        public Task<List<BlogPost>> FilterBlogPostByCategory(int? categoryId);

        public IEnumerable<BlogPost> SearchBlogPost(string searchString);
        public Task DeleteBlogPostAsync(int id);
    }
}
