using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechTalkBlog.Data;
using TechTalkBlog.Models;
using TechTalkBlog.Services.Interfaces;

namespace TechTalkBlog.Services
{
    public class BlogService : IBlogService
    {
        private readonly ApplicationDbContext _context;
        public BlogService(ApplicationDbContext context)
        {
            _context = context;
        }

        #region GetBlogPosts
        public async Task<IEnumerable<BlogPost>> GetAllBlogPostsAsync(int? tagId)
        {

            try
            {

                List<BlogPost> blogPosts = new();

                blogPosts = await _context.Posts.Include(b => b.Tags)
                                                .Include(b => b.Category)
                                                .Include(b => b.Comments)
                                                .ToListAsync();
                List<BlogPost> selectedBlogPosts = new();
                foreach (var blogPost in blogPosts)
                {
                    if (blogPost.IsDeleted != true && blogPost.IsPublished == true)
                    {
                        selectedBlogPosts.Add(blogPost);
                    }
                }
                return selectedBlogPosts;

            }
            catch (Exception)
            {

                throw;
            }

        }

        #endregion

        #region GetArchivedBlogPosts
        public async Task<IEnumerable<BlogPost>> GetAllArchivedBlogPostsAsync(int? tagId)
        {

            try
            {

                List<BlogPost> blogPosts = new();

                blogPosts = await _context.Posts.Include(b => b.Tags)
                                                .Include(b => b.Category)
                                                .Include(b => b.Comments)
                                                .ToListAsync();
                List<BlogPost> selectedBlogPosts = new();
                foreach (var blogPost in blogPosts)
                {
                    if (blogPost.IsDeleted == true && blogPost.IsPublished == true || blogPost.IsPublished == false)
                    {
                        selectedBlogPosts.Add(blogPost);
                    }
                }
                return selectedBlogPosts;

            }
            catch (Exception)
            {

                throw;
            }

        }

        #endregion



        #region GetBlogDetails
        public async Task<BlogPost> GetBlogDetailsAsync(int? id)
        {
            try
            {
                BlogPost? blogPost = await _context.Posts
                    .Include(b => b.Category)
                    .Include(b => b.Comments)
                        .ThenInclude(b => b.Author)
                    .Include(b => b.Tags)
                    .FirstOrDefaultAsync(m => m.Id == id);


                return blogPost!;

            }
            catch (Exception)
            {

                throw;
            }

        }
        #endregion

        public async Task<BlogPost> GetBlogByIdAsync(int? id)
        {
             BlogPost? blogPost = await _context.Posts
                    .Include(b => b.Category)
                    .Include(b => b.Comments)
                        .ThenInclude(b => b.Author)
                    .FirstOrDefaultAsync(m => m.Id == id);

            return blogPost!;
        }


        public async Task UpdateBlogPostAsync(BlogPost blogPost)
        {
            try
            {
                _context.Posts.Update(blogPost);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        #region Create(BlogPost blogPost, IEnumerable<int> selected)
        public async Task CreateBlogPostAsync(BlogPost blogPost, IEnumerable<int> selected)
        {
            try
            {
                

                _context.Add(blogPost);
                await _context.SaveChangesAsync();

                foreach (int tagId in selected)
                {
                    Tag? tag = await _context.Tags.FindAsync(tagId);
                    if (blogPost != null && tag != null)
                    {
                        blogPost.Tags.Add(tag);
                    }
                }
                await _context.SaveChangesAsync();

                

            }
            catch (Exception)
            {

                throw;
            }


        }
        #endregion


        #region Edit(BlogPost blogPost, IEnumerable<int> selected)
        public async Task<BlogPost> EditBlogPostAsync(BlogPost blogPost, IEnumerable<int> selected)
        {
            try
            {

                
                // Image files service
                _context.Update(blogPost);
                await _context.SaveChangesAsync();

                return blogPost;

            }
            catch (Exception)
            {

                throw;
            }

        }
        #endregion

        public async Task<BlogPost> DeleteBlogPostAsync_Get(int? id)
        {
            try
            {
                var blogPost = await _context.Posts
                    .Include(b => b.Category)
                    .FirstOrDefaultAsync(m => m.Id == id);

                return blogPost;

            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<BlogPost> DeleteBlogPostAsync_Post(int id)
        {
            throw new NotImplementedException();



        }
    }
}
