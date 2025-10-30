using System.Collections.Generic;
using TechTalkBlog.Models;

namespace TechTalkBlog.ViewModels
{
    public class UserWithRolesViewModel
    {
        public BlogUser User { get; set; } = new();
        public List<string> Roles { get; set; } = new();
    }
}
