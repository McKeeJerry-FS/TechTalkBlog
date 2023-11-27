using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechTalkBlog.Data;
using TechTalkBlog.Models;

namespace TechTalkBlog.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogPostsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BlogPostsController> _logger;

        public BlogPostsController(ApplicationDbContext context, 
                                   ILogger<BlogPostsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/BlogPosts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BlogPost>>> GetPosts()
        {
          if (_context.Posts == null)
          {
              return NotFound();
          }
            return await _context.Posts.ToListAsync();
        }

        // GET: api/BlogPosts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BlogPost>> GetBlogPost(int id)
        {
          if (_context.Posts == null)
          {
              return NotFound();
          }
            var blogPost = await _context.Posts.FindAsync(id);

            if (blogPost == null)
            {
                return NotFound();
            }

            return blogPost;
        }

        // PUT: api/BlogPosts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutBlogPost(int id, BlogPost blogPost)
        //{
        //    if (id != blogPost.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(blogPost).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!BlogPostExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        // POST: api/BlogPosts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost]
        //public async Task<ActionResult<BlogPost>> PostBlogPost(BlogPost blogPost)
        //{
        //  if (_context.Posts == null)
        //  {
        //      return Problem("Entity set 'ApplicationDbContext.Posts'  is null.");
        //  }
        //    _context.Posts.Add(blogPost);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetBlogPost", new { id = blogPost.Id }, blogPost);
        //}

        // DELETE: api/BlogPosts/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteBlogPost(int id)
        //{
        //    if (_context.Posts == null)
        //    {
        //        return NotFound();
        //    }
        //    var blogPost = await _context.Posts.FindAsync(id);
        //    if (blogPost == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Posts.Remove(blogPost);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        //GET:api/PortfolioBlogs
        [HttpGet]
        [Route("portfolio/{count}")]
        public async Task<ActionResult<IEnumerable<BlogPost>>> GetPortfolioBlogPosts(int? count)
        {
            if(_context.Posts == null || count == null)
            {
                _logger.LogWarning("NOT FOUND!!!");
                return NotFound();
            }

            IEnumerable<BlogPost>? result = await _context.Posts.Take(count.Value).ToListAsync();

            if(result.Any())
            {
                return Ok(result);
            }
            _logger.LogWarning("ERROR: BAD REQUEST ***from API/GetPortfolioBlogPosts***");
            return BadRequest();
        }






        private bool BlogPostExists(int id)
        {
            return (_context.Posts?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
