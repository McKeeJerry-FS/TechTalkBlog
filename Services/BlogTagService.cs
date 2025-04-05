using Microsoft.EntityFrameworkCore;
using GrowBlog.Data;
using GrowBlog.Models;
using GrowBlog.Services.Interfaces;

namespace GrowBlog.Services
{
    public class BlogTagService : IBlogTagService
    {

        private readonly ApplicationDbContext _context;

        public BlogTagService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddTagsToBlogPostAsync(IEnumerable<int> tagIds, int blogPostId)
        {
            try
            {
                BlogPost? blogPost = await _context.Posts
                                                 .Include(b => b.Tags)
                                                 .FirstOrDefaultAsync(b => b.Id == blogPostId);

                foreach (int tagId in tagIds)
                {
                    Tag? tag = await _context.Tags.FindAsync(tagId);

                    if (blogPost != null && tag != null)
                    {
                        blogPost.Tags!.Add(tag);
                    }
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task RemoveTagsFromBlogPostAsync(int blogPostId)
        {
            try
            {
                BlogPost? blogPost = await _context.Posts
                                                 .Include(c => c.Tags)
                                                 .FirstOrDefaultAsync(c => c.Id == blogPostId);

                if (blogPost != null)
                {
                    blogPost.Tags!.Clear();
                    _context.Update(blogPost);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
