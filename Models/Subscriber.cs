using System;
using System.ComponentModel.DataAnnotations;

namespace TechTalkBlog.Models
{
    public class Subscriber
    {
        public int Id { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

        public DateTime SubscribedOn { get; set; } = DateTime.UtcNow;
    }
}
