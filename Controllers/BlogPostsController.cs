using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechTalkBlog.Data;
using TechTalkBlog.Helpers;
using TechTalkBlog.Models;
using TechTalkBlog.Services;
using TechTalkBlog.Services.Interfaces;
using X.PagedList;

namespace TechTalkBlog.Controllers
{
    
    public class BlogPostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BlogUser> _userManager;
        private readonly ILogger<BlogPostsController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IBlogTagService _blogTagService;
        private readonly IBlogService _blogService;
        private readonly IImageService _imageService;
        private readonly IEmailSender _emailService;


        public BlogPostsController(ApplicationDbContext context,                                   
                                    IBlogTagService blogTagService,
                                    IBlogService blogService,
                                    IImageService imageService,
                                    UserManager<BlogUser> userManager,
                                    ILogger<BlogPostsController> logger,
                                    IConfiguration configuration,
                                    IEmailSender emailService)
        {
            _context = context;
            _blogTagService = blogTagService;
            _blogService = blogService;
            _imageService = imageService;
            _userManager = userManager;
            _logger = logger;
            _configuration = configuration;
            _emailService = emailService;
        }

        #region Task<IActionResult> ContactMe()
        [Authorize]
        public async Task<IActionResult> ContactMe()
        {
            string? blogUserId = _userManager.GetUserId(User);
            if (blogUserId == null)
            {
                return NotFound();
            }
            BlogUser? blogUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == blogUserId);
            return View(blogUser);
        }

        #endregion

        #region Task<IActionResult> ContactMe([Bind("FirstName, LastName, Email")] BlogUser blogUser, string? message)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ContactMe([Bind("FirstName, LastName, Email")] BlogUser blogUser, string? message)
        {
            string? swalMessage = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    string? adminEmail = _configuration["AdminEmail"] ?? Environment.GetEnvironmentVariable("AdminEmail");
                    await _emailService.SendEmailAsync(adminEmail!, $"Contact Me Mesage from - {blogUser.FullName}", message!);
                    swalMessage = "Email Sent Successfully";
                }
                catch (Exception)
                {

                    throw;
                }
                swalMessage = "Error: Unable to send message";
            }
            return RedirectToAction("Index", new { swalMessage });
        }

        #endregion

        #region Task<IActionResult> Index(int? categoryId, int? pageNum)
        // GET: BlogPosts
        [AllowAnonymous]
        public async Task<IActionResult> Index(int? categoryId, int? pageNum)
        {

            IPagedList<BlogPost> blogPosts;
            // new service included
            int pageSize = 4;
            int page = pageNum ?? 1;



            if (categoryId == null)
            {
                // normal operation
                blogPosts = await (await _blogService.GetBlogPostsAsync()).ToPagedListAsync(page, pageSize);
               
            }
            else
            {
                // filtered by category
                blogPosts = await (await _blogService.FilterBlogPostByCategory(categoryId)).ToPagedListAsync(page, pageSize);
            }

            // make service call
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["Tags"] = new MultiSelectList(_context.Tags, "Id", "Name");
            return View(blogPosts);
        }

        #endregion

        #region Task<IActionResult> Archived(int? tagId)
        // GET: BlogPosts
        public async Task<IActionResult> Archived(int? tagId)
        {

            IEnumerable<BlogPost> blogPosts;
            // new service included

            blogPosts = await _blogService.GetAllArchivedBlogPostsAsync(tagId);

            // make service call
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["Tags"] = new MultiSelectList(_context.Tags, "Id", "Name");
            return View(blogPosts);
        }

        #endregion

        #region Task<IActionResult> Draft(int? tagId)
        // GET: BlogPosts
        public async Task<IActionResult> Draft(int? tagId)
        {

            IEnumerable<BlogPost> blogPosts;
            // new service included

            blogPosts = await _blogService.GetAllDraftBlogPostsAsync(tagId);

            // make service call
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["Tags"] = new MultiSelectList(_context.Tags, "Id", "Name");
            return View(blogPosts);
        }

        #endregion

        #region Task<IActionResult> Popular(int? pageNum)
        [AllowAnonymous]
        public async Task<IActionResult> Popular(int? pageNum)
        {
            int pageSize = 4;
            int page = pageNum ?? 1;

            IPagedList<BlogPost> popularBlogs;
            popularBlogs = await (await _blogService.GetPopularBlogsAsync()).ToPagedListAsync(page, pageSize);

            // make service call
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["Tags"] = new MultiSelectList(_context.Tags, "Id", "Name");
            return View(popularBlogs);
        }

        #endregion

        #region Task<IActionResult> Popular(int? pageNum)
        [Authorize]
        public async Task<IActionResult> Favorites(int? pageNum)
        {
            int pageSize = 4;
            int page = pageNum ?? 1;

            IPagedList<BlogPost> favoriteBlogs;
            favoriteBlogs = await (await _blogService.GetPopularBlogsAsync()).ToPagedListAsync(page, pageSize);

            // make service call
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["Tags"] = new MultiSelectList(_context.Tags, "Id", "Name");
            return View(favoriteBlogs);
        }

        #endregion

        #region Task<IActionResult> Details(string? slug)
        // GET: BlogPosts/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(string? slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return NotFound();
            }

            // New BlogService in Use
            var blogPost = await _blogService.GetBlogBySlugAsync(slug);

            if (blogPost == null)
            {
                return NotFound();
            }

            return View(blogPost);
        }

        #endregion

        #region Create()
        // GET: BlogPosts/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {

            // make a service call
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["Tags"] = new SelectList(_context.Tags, "Id", "Name");
            return View();
        }

        #endregion

        #region Task<IActionResult> Create([Bind("Id,Title,Abstract,Content,IsPublished,CategoryId,Tags,ImageFile,ImageData,ImageType")] BlogPost blogPost, IEnumerable<int> selected)
        // POST: BlogPosts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Abstract,Content,IsPublished,CategoryId,Tags,ImageFile,ImageData,ImageType")] BlogPost blogPost, IEnumerable<int> selected)
        {
            ModelState.Remove("BlogUserId");
            ModelState.Remove("Slug");

            if (ModelState.IsValid)
            {
                string? newSlug = StringHelper.BlogPostSlug(blogPost.Title);
                if (!await _blogService.IsValidSlugAsync(newSlug, blogPost.Id))
                {
                    ModelState.AddModelError("Title", "A similar Title/Slug is already in use. Please choose a different title.");

                    // make a service call
                    ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", blogPost.CategoryId);
                    ViewData["Tags"] = new MultiSelectList(_context.Tags, "Id", "Name", blogPost.Tags);

                    return View(blogPost);
                }

                blogPost.Slug = newSlug;
                blogPost.CreatedDate = DateTimeOffset.Now.ToUniversalTime();

                if (blogPost.ImageFile != null)
                {
                    // use image service
                    blogPost.ImageData = await _imageService.ConvertFileToByteArrayAsynC(blogPost.ImageFile);
                    blogPost.ImageType = blogPost.ImageFile.ContentType;
                }

                // Handle Tags
                if (selected != null)
                {
                    // Remove the current categories
                    await _blogTagService.RemoveTagsFromBlogPostAsync(blogPost.Id);
                    // Add the updated categories
                    await _blogTagService.AddTagsToBlogPostAsync(selected, blogPost.Id);
                }

                // new BlogPost Service utilized
                await _blogService.CreateBlogPostAsync(blogPost, selected!);

                return RedirectToAction(nameof(Index));
            }
            IEnumerable<int> currentTags = blogPost.Tags!.Select(c => c.Id);

            // make a service call
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", blogPost.CategoryId);
            ViewData["Tags"] = new MultiSelectList(_context.Tags, "Id", "Name", blogPost.Tags);

            return View(blogPost);
        }

        #endregion

        #region Task<IActionResult> Edit(int? id)
        // GET: BlogPosts/Edit/5
        [Authorize(Roles = "Admin, Moderator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            BlogPost blogPost = await _blogService.GetBlogDetailsAsync(id);
            if (blogPost == null)
            {
                return NotFound();
            }

            IEnumerable<int> currentTags = blogPost.Tags!.Select(c => c.Id);

            // make a service call
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", blogPost.CategoryId);
            ViewData["Tags"] = new MultiSelectList(_context.Tags, "Id", "Name", currentTags);

            return View(blogPost);
        }

        #endregion

        #region Task<IActionResult> Edit(int id, [Bind("Id,Title,BlogUserId,Abstract,Content,CreatedDate,UpdatedDate,Slug,IsArchived,IsPublished,CategoryId,ImageFile,ImageData,ImageType")] BlogPost blogPost, IEnumerable<int> selected)
        // POST: BlogPosts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin, Moderator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,BlogUserId,Abstract,Content,CreatedDate,UpdatedDate,Slug,IsArchived,IsPublished,CategoryId,ImageFile,ImageData,ImageType")] BlogPost blogPost, IEnumerable<int> selected)
        {
            if (id != blogPost.Id)
            {
                return NotFound();
            }

            ModelState.Remove("Slug");

            if (ModelState.IsValid)
            {
                try
                {
                    string? newSlug = StringHelper.BlogPostSlug(blogPost.Title);
                    if (!await _blogService.IsValidSlugAsync(newSlug, blogPost.Id))
                    {
                        ModelState.AddModelError("Title", "A similar Title/Slug is already in use. Please choose a different title.");

                        // make a service call
                        ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", blogPost.CategoryId);
                        ViewData["Tags"] = new MultiSelectList(_context.Tags, "Id", "Name", blogPost.Tags);

                        return View(blogPost);
                    }

                    blogPost.Slug = newSlug;

                    blogPost.UpdatedDate = DateTimeOffset.Now.ToUniversalTime();
                    if (blogPost.ImageFile != null)
                    {
                        blogPost.ImageData = await _imageService.ConvertFileToByteArrayAsynC(blogPost.ImageFile);
                        blogPost.ImageType = blogPost.ImageFile.ContentType;
                    }

                    // Future Enhancement: If blog post is brought back from archive, add a Revival Date to show the date the post returns

                    // new BlogService in use
                    blogPost = await _blogService.EditBlogPostAsync(blogPost, selected);

                    // Handle Tags
                    if (selected != null)
                    {
                        // Remove the current categories
                        await _blogTagService.RemoveTagsFromBlogPostAsync(blogPost.Id);
                        // Add the updated categories
                        await _blogTagService.AddTagsToBlogPostAsync(selected, blogPost.Id);
                    }

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await BlogPostExistsAsync(blogPost.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            // make a service call
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", blogPost.CategoryId);
            ViewData["Tags"] = new MultiSelectList(_context.Tags, "Id", "Name", blogPost.Tags);
            return View(blogPost);
        }

        #endregion

        #region Task<IActionResult> Archive(int? id)
        // GET: BlogPosts/Archive/5
        [Authorize(Roles = "Admin, Moderator")]
        public async Task<IActionResult> Archive(int? id) 
        {
            if (id == null)
            {
                return NotFound();
            }


            var blogPost = await _blogService.GetBlogDetailsAsync(id);
            
            if (blogPost == null)
            {
                return NotFound();
            }
            

            return View(blogPost);
        }
        #endregion

        #region Task<IActionResult> ArchiveConfirmed(int id)
        // POST: BlogPosts/Delete/5
        [Authorize(Roles = "Admin, Moderator")]
        [HttpPost, ActionName("Archive")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ArchiveConfirmed(int id)
        {

            var blogPost = await _blogService.GetBlogByIdAsync(id);
            if (blogPost != null)
            {
                //_context.Posts.Remove(blogPost);
                blogPost.IsArchived = true;
                await _blogService.UpdateBlogPostAsync(blogPost);
                _logger.LogInformation(message: "Blog successfully archived");
                
            }

            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region Task<IActionResult> SearchIndex(string? searchString, int? pageNum)
        // Search Feature
        [AllowAnonymous]
        public async Task<IActionResult> SearchIndex(string? searchString, int? pageNum)
        {


            if (string.IsNullOrWhiteSpace(searchString))
            {
                return RedirectToAction(nameof(Index));
            }
            int pageSize = 4;
            int page = pageNum ?? 1;

            IPagedList<BlogPost> blogPosts = await _blogService.SearchBlogPost(searchString).ToPagedListAsync(page, pageSize);
            ViewData["Search"] = searchString;
            return View(nameof(Index), blogPosts);

        }

        #endregion

        #region Task<IActionResult> Delete(int? id)
        // Delete
        // GET: BlogPosts/Archive/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            
            BlogPost? blogPost = await _blogService.GetBlogDetailsAsync(id);

            if (blogPost == null)
            {
                return NotFound();
            }

            return View(blogPost);
        }

        #endregion

        #region Task<IActionResult> DeleteConfirmed(int id)
        // POST: BlogPosts/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            var blogPost = await _blogService.GetBlogByIdAsync(id);
            if (blogPost != null)
            {
                //_context.Posts.Remove(blogPost);
                blogPost.IsArchived = true;
                await _blogService.DeleteBlogPostAsync(id);
            }

            return RedirectToAction(nameof(Index));
        }

        #endregion

        public async Task<IActionResult> LikeBlogPost(int? blogPostId, string? blogUserId)
        {
            BlogUser? blogUser = await _context.Users.Include(u => u.Likes).FirstOrDefaultAsync(u => u.Id == blogUserId);
            bool result = false;
            BlogLike? blogLike = new();

            if (blogUser != null && blogPostId != null)
            {
                if (!blogUser.Likes.Any(bl => bl.BlogPostId == blogPostId))
                {
                    result = true;
                }
                else
                {

                }
                result = blogLike.IsLiked;
                await _context.SaveChangesAsync();
            }
            return Json(new
            {
                isLiked = result,
                count = _context.BlogLikes.Where(bl => bl.BlogPostId == blogPostId && bl.IsLiked == true).Count()

            });
        }
        //*************************************\\
        #region Admin Only Functions

        #region Task<IActionResult> AuthorArea(int? pageNum)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AuthorArea(int? pageNum)
        {
            int pageSize = 3;
            int page = pageNum ?? 1;

            IPagedList<BlogPost> blogPosts = await (await _blogService.GetAllBlogPostsAsync())
                                                                     .ToPagedListAsync(page, pageSize);
            return View(blogPosts);
        }

        #endregion

        #region Task<IActionResult> AdminArchive(int? id)
        [Authorize(Roles = "Admin")]
        // GET: BlogPosts/Delete/5
        public async Task<IActionResult> AdminArchive(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            BlogPost? blogPost = await _blogService.GetBlogByIdAsync(id);

            //var blogPost = await _blogService.GetBlogPostAsync(id);

            //    .FirstOrDefaultAsync(m => m.Id == id);

            if (blogPost == null)
            {
                return NotFound();
            }

            blogPost.IsArchived = true;

            await _blogService.UpdateBlogPostAsync(blogPost);

            return RedirectToAction(nameof(AuthorArea));
        }

        #endregion

        #region Task<IActionResult> UnArchive(int id)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Unarchive(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            BlogPost? blogPost = await _blogService.GetBlogByIdAsync(id);
            if (blogPost != null)
            {
                blogPost.IsArchived = false;

                await _blogService.UpdateBlogPostAsync(blogPost);
            }

            return RedirectToAction(nameof(AuthorArea));
        }

        #endregion

        #region Task<IActionResult> Publish(int? id)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Publish(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            BlogPost? blogPost = await _blogService.GetBlogByIdAsync(id);
            if (blogPost == null)
            {
                return NotFound();
            }

            blogPost.IsPublished = true;
            await _blogService.UpdateBlogPostAsync(blogPost);
            return RedirectToAction(nameof(AuthorArea));

        }
        #endregion

        #region Task<IActionResult> Unpublish(int? id)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Unpublish(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            BlogPost? blogPost = await _blogService.GetBlogByIdAsync(id);
            if (blogPost == null)
            {
                return NotFound();
            }
            blogPost.IsPublished = false;
            await _blogService.UpdateBlogPostAsync(blogPost);
            return RedirectToAction(nameof(AuthorArea));
        }

        #endregion

        #endregion
        //*************************************\\

        private async Task<bool> BlogPostExistsAsync(int id)
        {
          return ((await _blogService.GetBlogPostsAsync()).Any(e => e.Id == id));
        }
    }
}

