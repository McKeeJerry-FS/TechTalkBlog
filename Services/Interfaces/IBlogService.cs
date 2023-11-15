﻿using TechTalkBlog.Models;

namespace TechTalkBlog.Services.Interfaces
{
    public interface IBlogService
    {
        public Task<List<BlogPost>> GetAllBlogPostsAsync(int? tagId);
        public Task<BlogPost> GetBlogDetailsAsync(int? id);

        public Task<BlogPost> CreateBlogPostAsync(BlogPost blogPost, IEnumerable<int> selected);

        public Task<BlogPost> EditBlogPostAsync(BlogPost blogPost, IEnumerable<int> selected);
    }
}