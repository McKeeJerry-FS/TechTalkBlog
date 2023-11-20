using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechTalkBlog.Data;
using TechTalkBlog.Models;
using TechTalkBlog.Services;
using TechTalkBlog.Services.Interfaces;
using X.PagedList;

namespace TechTalkBlog.Controllers
{
    [Authorize]
    public class BlogPostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IBlogTagService _blogTagService;
        private readonly IBlogService _blogService;
        private readonly IImageService _imageService;

        public BlogPostsController(ApplicationDbContext context,                                   
                                    IBlogTagService blogTagService,
                                    IBlogService blogService,
                                    IImageService imageService)
        {
            _context = context;
            _blogTagService = blogTagService;
            _blogService = blogService;
            _imageService = imageService;
        }

        // GET: BlogPosts
        public async Task<IActionResult> Index(int? categoryId, int? pageNum)
        {

            IPagedList<BlogPost> blogPosts;
            // new service included
            int pageSize = 4;
            int page = pageNum ?? 1;



            if (categoryId == null)
            {
                // normal operation
                blogPosts = await(await _blogService.GetAllBlogPostsAsync()).ToPagedListAsync(page, pageSize);
            }
            else
            {
                // filtered by category
                blogPosts = await(await _blogService.FilterBlogPostByCategory(categoryId)).ToPagedListAsync(page, pageSize);
            }

            // make service call
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["Tags"] = new MultiSelectList(_context.Tags, "Id", "Name");
            return View(blogPosts);
        }

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


        // GET: BlogPosts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // New BlogService in Use
            var blogPost = await _blogService.GetBlogDetailsAsync(id);

            if (blogPost == null)
            {
                return NotFound();
            }

            return View(blogPost);
        }

        // GET: BlogPosts/Create
        public IActionResult Create()
        {

            // make a service call
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["Tags"] = new SelectList(_context.Tags, "Id", "Name");
            return View();
        }

        // POST: BlogPosts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Abstract,Content,IsPublished,CategoryId,Tags,ImageFile,ImageData,ImageType")] BlogPost blogPost, IEnumerable<int> selected)
        {
            ModelState.Remove("BlogUserId");


            if (ModelState.IsValid)
            {
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

        // GET: BlogPosts/Edit/5
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

        // POST: BlogPosts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,BlogUserId,Abstract,Content,CreatedDate,UpdatedDate,Slug,IsArchived,IsPublished,CategoryId,ImageFile,ImageData,ImageType")] BlogPost blogPost, IEnumerable<int> selected)
        {
            if (id != blogPost.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
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

        // GET: BlogPosts/Archive/5
        public async Task<IActionResult> Archive(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var blogPost = await _context.Posts
            //    .Include(b => b.Category)
            //    .FirstOrDefaultAsync(m => m.Id == id);

            var blogPost = await _blogService.GetBlogDetailsAsync(id);

            if (blogPost == null)
            {
                return NotFound();
            }

            return View(blogPost);
        }

        // POST: BlogPosts/Delete/5
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
            }
            
            return RedirectToAction(nameof(Index));
        }


        // Search Feature
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

        // Delete
        // GET: BlogPosts/Archive/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var blogPost = await _context.Posts
            //    .Include(b => b.Category)
            //    .FirstOrDefaultAsync(m => m.Id == id);

            var blogPost = await _blogService.GetBlogDetailsAsync(id);

            if (blogPost == null)
            {
                return NotFound();
            }

            return View(blogPost);
        }

        // POST: BlogPosts/Delete/5
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



        private async Task<bool> BlogPostExistsAsync(int id)
        {
          return ((await _blogService.GetAllBlogPostsAsync()).Any(e => e.Id == id));
        }
    }
}
