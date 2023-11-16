using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechTalkBlog.Models
{
    public class BlogPost
    {
        public int Id { get; set; }


        [Required]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at most {1} characters", MinimumLength = 2)]
        public string? Title { get; set; }

        [StringLength(600, ErrorMessage = "The {0} must be at least {2} and at most {1} characters", MinimumLength = 2)]
        public string? Abstract { get; set; }

        public string? Content { get; set; }
        
        [Display(Name = "Created Date")]
        public DateTimeOffset CreatedDate { get; set; }

        [Display(Name = "Updated Date")]
        public DateTimeOffset UpdatedDate { get; set; }

        [Display(Name = "Revival Date")]
        public DateTimeOffset RevivalDate { get; set; }

        public string? Slug { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsPublished { get; set; }
        public int CategoryId { get; set; } // Foreign Key


        // Image Properties
        [NotMapped]
        public IFormFile? ImageFile { get; set; }
        public byte[]? ImageData { get; set; }
        public string? ImageType { get; set; }

        // Navigation (Singular)
        public virtual Category? Category { get; set; }


        // Navigation Collection
        public virtual ICollection<Comment>? Comments { get; set; } = new HashSet<Comment>();
        public virtual ICollection<Tag>? Tags { get; set; } = new HashSet<Tag>();
    }
}
