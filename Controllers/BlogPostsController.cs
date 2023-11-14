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

namespace TechTalkBlog.Controllers
{
    [Authorize]
    public class BlogPostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IBlogTagService _blogTagService;
        private readonly IBlogService _blogService;

        public BlogPostsController(ApplicationDbContext context,                                   
                                    IBlogTagService blogTagService,
                                    IBlogService blogService)
        {
            _context = context;
            _blogTagService = blogTagService;
            _blogService = blogService;
        }

        // GET: BlogPosts
        public async Task<IActionResult> Index(int? tagId)
        {

            List<BlogPost> blogPosts = new();
            // new service included
            blogPosts = await _blogService.GetBlogPosts(tagId);


            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["Tags"] = new MultiSelectList(_context.Tags, "Id", "Name");
            return View(blogPosts);
        }

        // GET: BlogPosts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var blogPost = await _blogService.GetBlogDetails(id);

            if (blogPost == null)
            {
                return NotFound();
            }

            return View(blogPost);
        }

        // GET: BlogPosts/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["Tags"] = new MultiSelectList(_context.Tags, "Id", "Name");
            return View();
        }

        // POST: BlogPosts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Abstract,Content,IsPublished,CategoryId,Tags,ImageFile")] BlogPost blogPost, IEnumerable<int> selected)
        {
            ModelState.Remove("BlogUserId");


            if (ModelState.IsValid)
            {
                // new BlogPost Service utilized
                blogPost = await _blogService.Create(blogPost, selected);
                
                return RedirectToAction(nameof(Index));
            }
            IEnumerable<int> currentTags = blogPost.Tags.Select(c => c.Id);

            
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", blogPost.CategoryId);
            ViewData["Tags"] = new MultiSelectList(_context.Tags, "Id", "Name", blogPost.Tags);

            return View(blogPost);
        }

        // GET: BlogPosts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var blogPost = await _context.Posts.FindAsync(id);
            if (blogPost == null)
            {
                return NotFound();
            }

            IEnumerable<int> currentTags = blogPost.Tags.Select(c => c.Id);

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", blogPost.CategoryId);
            ViewData["Tags"] = new SelectList(_context.Tags, "Id", "Name", currentTags);

            return View(blogPost);
        }

        // POST: BlogPosts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,BlogUserId,Abstract,Content,CreatedDate,UpdatedDate,Slug,IsDeleted,IsPublished,CategoryId,ImageFile,ImageData,ImageType")] BlogPost blogPost, IEnumerable<int> selected)
        {
            if (id != blogPost.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // new BlogService in use
                    blogPost = await _blogService.Edit(blogPost, selected);

                    // Handle categories
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
                    if (!BlogPostExists(blogPost.Id))
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", blogPost.CategoryId);
            ViewData["Tags"] = new MultiSelectList(_context.Tags, "Id", "Name", blogPost.Tags);
            return View(blogPost);
        }

        // GET: BlogPosts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var blogPost = await _context.Posts
                .Include(b => b.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
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
            if (_context.Posts == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Posts'  is null.");
            }
            var blogPost = await _context.Posts.FindAsync(id);
            if (blogPost != null)
            {
                _context.Posts.Remove(blogPost);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BlogPostExists(int id)
        {
          return (_context.Posts?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
