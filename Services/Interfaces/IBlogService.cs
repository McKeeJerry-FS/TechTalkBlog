using TechTalkBlog.Models;

namespace TechTalkBlog.Services.Interfaces
{
    public interface IBlogService
    {
        public Task<List<BlogPost>> GetBlogPosts(int? tagId);
        public Task<BlogPost> GetBlogDetails(int? id);

        public Task<BlogPost> Create(BlogPost blogPost, IEnumerable<int> selected);

        public Task<BlogPost> Edit(BlogPost blogPost, IEnumerable<int> selected);
    }
}
