using System.ComponentModel.DataAnnotations;

namespace TechTalkBlog.Models
{
    public class Comment
    {
        public int Id { get; set; }

        // Foreign Key
        [Required]
        public string? BlogUserId { get; set; }

        [Required]
        [StringLength(5000, ErrorMessage = "The {0} must be at least {2} and at most {1} charcters", MinimumLength = 2)]
        public string? Body { get; set; }

        [Display(Name = "Comment Created")]
        public DateTimeOffset? Created { get; set; }

        [Display(Name = "Comment Updated")]
        public DateTimeOffset? Updated { get; set; }

        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at most {1} characters", MinimumLength = 2)]
        public string? UpdateReason { get; set; }

        // Foreign Keys
        public int BlogPostId { get; set; }
        public string? AuthorId { get; set; }


        // Navigation Props
        public virtual BlogPost? BlogPost { get; set; }
        public virtual BlogUser? Author { get; set; }
    }
}
