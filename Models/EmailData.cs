using System.ComponentModel.DataAnnotations;

namespace GrowBlog.Models
{
    public class EmailData
    {
        [Required]
        public string? EmailAddress { get; set; }
        [Required]
        public string? EmailSubject { get; set; }
        [Required]
        public string? EmailBody { get; set; }



        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? GroupName { get; set; }
    }
}
