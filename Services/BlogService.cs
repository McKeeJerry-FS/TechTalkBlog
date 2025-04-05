using Microsoft.EntityFrameworkCore;
using TechTalkBlog.Data;
using TechTalkBlog.Models;
using TechTalkBlog.Services.Interfaces;

namespace TechTalkBlog.Services
{
    public class BlogService : IBlogService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BlogService> _logger;
        
        public BlogService(ApplicationDbContext context,
                            ILogger<BlogService> logger)
        {
            _context = context;
            _logger = logger;
            
        }




        #region GetBlogPosts
        public async Task<IEnumerable<BlogPost>> GetBlogPostsAsync()
        {

            try
            {

                List<BlogPost> blogPosts = new();

                blogPosts = await _context.Posts.Include(b => b.Tags)
                                                .Include(b => b.Category)
                                                .Include(b => b.Comments)
                                                .Include(b => b.Likes)
                                                .OrderByDescending(b => b.CreatedDate)
                                                .ToListAsync();
                List<BlogPost> selectedBlogPosts = new();
                foreach (var blogPost in blogPosts)
                {
                    if (blogPost.IsArchived != true && blogPost.IsPublished == true)
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

        #region GetAllBlogPosts
        public async Task<IEnumerable<BlogPost>> GetAllBlogPostsAsync()
        {

            try
            {

                List<BlogPost> blogPosts = new();

                blogPosts = await _context.Posts.Include(b => b.Tags)
                                                .Include(b => b.Category)
                                                .Include(b => b.Comments)
                                                .Include(b => b.Likes)
                                                .OrderByDescending(b => b.CreatedDate)
                                                .ToListAsync();
                return blogPosts;

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
                                                .OrderByDescending (b => b.CreatedDate)
                                                .ToListAsync();
                List<BlogPost> selectedBlogPosts = new();
                foreach (var blogPost in blogPosts)
                {
                    if (blogPost.IsArchived == true && blogPost.IsPublished == true || blogPost.IsPublished == false)
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

        #region GetDraftBlogPosts
        public async Task<IEnumerable<BlogPost>> GetAllDraftBlogPostsAsync(int? tagId)
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
                    if (blogPost.IsPublished == false && blogPost.IsArchived == false)
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
                    .Include(b => b.Likes)
                    .FirstOrDefaultAsync(m => m.Id == id);


                return blogPost!;

            }
            catch (Exception)
            {

                throw;
            }

        }
        #endregion

        #region Task<IEnumerable<BlogPost>> GetFavoriteBlogPostsAsync(string? blogUserId)
        public async Task<IEnumerable<BlogPost>> GetFavoriteBlogPostsAsync(string? blogUserId)
        {
            try
            {
                List<BlogPost> blogPosts = new();
                if (!string.IsNullOrEmpty(blogUserId))
                {
                    BlogUser? blogUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == blogUserId);
                    if (blogUser != null)
                    {
                        //List<int> blogPostIds = _context.BlogLikes.Where(bl => bl.BlogUserId == blogUserId && bl.IsLiked == true).Select(b => b.BlogPostId).ToList();
                        blogPosts = await _context.Posts.Where(b => b.Likes.Any(l => l.BlogUserId == blogUserId && l.IsLiked == true) &&
                                                                                    b.IsPublished == true &&
                                                                                    b.IsArchived == false)
                                                            .Include(b => b.Likes)
                                                            .Include(b => b.Comments)
                                                            .Include(b => b.Category)
                                                            .Include(b => b.Tags)
                                                            .OrderByDescending(b => b.CreatedDate)
                                                            .ToListAsync();
                    }
                }
                return blogPosts;
            }
            catch (Exception)
            {

                throw;
            }
        } 
        #endregion

        #region Task<BlogPost> GetBlogByIdAsync(int? id)
        public async Task<BlogPost> GetBlogByIdAsync(int? id)
        {
            BlogPost? blogPost = await _context.Posts
                   .Include(b => b.Category)
                   .Include(b => b.Comments)
                       .ThenInclude(b => b.Author)
                   .FirstOrDefaultAsync(m => m.Id == id);

            return blogPost!;
        }
        #endregion

        #region Task<BlogPost> GetBlogBySlugAsync(string? slug)
        public async Task<BlogPost> GetBlogBySlugAsync(string? slug)
        {
            BlogPost? blogPost = await _context.Posts
                   .Include(b => b.Category)
                   .Include(b => b.Comments)
                       .ThenInclude(b => b.Author)
                   .FirstOrDefaultAsync(m => m.Slug == slug && m.IsPublished == true && m.IsArchived == false);

            return blogPost!;


            
        }
        #endregion

        #region Task<IEnumerable<BlogPost>> GetPopularBlogs()
        public async Task<IEnumerable<BlogPost>> GetPopularBlogsAsync()
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
                    if (blogPost.IsArchived != true && blogPost.IsPublished == true)
                    {
                        selectedBlogPosts.Add(blogPost);
                    }
                }

                return selectedBlogPosts.OrderByDescending(b => b.Comments!.Count());

            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region Task UpdateBlogPostAsync(BlogPost blogPost)
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
        #endregion

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

        #region Task<BlogPost> DeleteBlogPostAsync_Get(int? id)
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
        #endregion

        #region DeleteBlogPostAsync(int id)
        public async Task DeleteBlogPostAsync(int id)
        {
            try
            {
                var blogpost = await _context.Posts.FirstOrDefaultAsync(b => b.Id == id);
                if (blogpost != null)
                {
                    _context.Posts.Remove(blogpost);
                }

                await _context.SaveChangesAsync();


            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion

        #region FilterBlogPostByCategory(int? categoryId)
        public async Task<List<BlogPost>> FilterBlogPostByCategory(int? categoryId)
        {
            try
            {
                List<BlogPost> blogPost = new();

                Category? category = new();
                category = await _context.Categories.Include(c => c.BlogPosts)
                                                    .FirstOrDefaultAsync(c => c.Id == categoryId);
                if (category != null)
                {
                    blogPost = category.BlogPosts!.ToList();
                }

                return blogPost;


            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region SearchBlogPost(string searchString)
        public IEnumerable<BlogPost> SearchBlogPost(string searchString)
        {
            try
            {
                searchString = searchString.Trim().ToLower();
                IEnumerable<BlogPost> blogPosts = _context.Posts
                                                          .Where(b => b.IsPublished == true && b.IsArchived == false)
                                                          .Where(b => b.Title!.ToLower().Contains(searchString)
                                                                   || (!string.IsNullOrEmpty(b.Abstract) && b.Abstract.ToLower().Contains(searchString))
                                                                   || b.Content!.ToLower().Contains(searchString)
                                                                   || b.Tags.Any(t => t.Name!.ToLower().Contains(searchString))
                                                                   || b.Category!.Name!.ToLower().Contains(searchString)
                                                                   || b.Comments.Any(c => c.Body!.ToLower().Contains(searchString)
                                                                                       || c.Author!.FirstName!.ToLower().Contains(searchString)
                                                                                       || c.Author!.LastName!.ToLower().Contains(searchString))
                                                                 )
                                                           .Include(b => b.Category)
                                                           .Include(b => b.Tags)
                                                           .Include(b => b.Comments)
                                                            .ThenInclude(b => b.Author)
                                                           .AsNoTracking()
                                                           .OrderByDescending(b => b.CreatedDate)
                                                           .AsEnumerable();

                return blogPosts;
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region GetCategoriesAsync()
        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            IEnumerable<Category> categories = await _context.Categories.ToListAsync();
            return categories;
        }
        #endregion

        #region Task<bool> IsValidSlugAsync(string? slug, int? blogPostId)
        public async Task<bool> IsValidSlugAsync(string? slug, int? blogPostId)
        {
            try
            {
                if (blogPostId == null || blogPostId == 0)
                {
                    // This indicates that a new blog post is being created
                    bool isSlug = !await _context.Posts.AnyAsync(b => b.Slug == slug);
                    return isSlug;
                }
                else
                {
                    // Editing an existing BlogPost 
                    BlogPost? blogPost = await _context.Posts.AsNoTracking()
                                                             .FirstOrDefaultAsync(b => b.Id == blogPostId);
                    string? oldSlug = blogPost?.Slug;
                    if (!string.Equals(oldSlug, slug))
                    {
                        return !await _context.Posts.AnyAsync(b => b.Id != blogPost!.Id && b.Slug == slug);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {

                throw;
            }
        } 
        #endregion

        #region Task<bool> UserLikedBlogAsync(int blogPostId, string blogUserId)
        public async Task<bool> UserLikedBlogAsync(int blogPostId, string blogUserId)
        {
            try
            {
                return await _context.BlogLikes.AnyAsync(bl => bl.BlogPostId == blogPostId
                                                            && bl.IsLiked == true
                                                            && bl.BlogUserId == blogUserId);
            }
            catch (Exception)
            {
                _logger.LogInformation("Error: No BlogLikes found");
                throw;
            }
        }
        #endregion    
    }
}
