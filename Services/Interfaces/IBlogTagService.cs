namespace TechTalkBlog.Services.Interfaces
{
    public interface IBlogTagService
    {
        public Task AddTagsToBlogPostAsync(IEnumerable<int> tagId, int blogPostId);
        public Task RemoveTagsFromBlogPostAsync(int blogPostId);
    }
}
