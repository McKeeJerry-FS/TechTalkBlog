﻿using Microsoft.AspNetCore.Mvc;
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
                    if (blogPost.IsDeleted != true)
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
                var blogPost = await _context.Posts
                    .Include(b => b.Category)
                    .Include(b => b.Comments).ThenInclude(b => b.Author)
                    .FirstOrDefaultAsync(m => m.Id == id);


                return blogPost;

            }
            catch (Exception)
            {

                throw;
            }

        }
        #endregion


        #region Create(BlogPost blogPost, IEnumerable<int> selected)
        public async Task<BlogPost> CreateBlogPostAsync(BlogPost blogPost, IEnumerable<int> selected)
        {
            try
            {
                blogPost.CreatedDate = DateTimeOffset.Now.ToUniversalTime();

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

                return blogPost;

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

                blogPost.UpdatedDate = DateTimeOffset.Now.ToUniversalTime();
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
