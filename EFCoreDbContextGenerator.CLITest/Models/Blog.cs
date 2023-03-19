using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations; 

namespace EFCoreDbContextGenerator.CLITest.Models
{
    [Table("Blogs")]
    public class Blog
    {
        public int BlogId { get; set; }

        [Required]
        public string Url { get; set; }
    }
}
