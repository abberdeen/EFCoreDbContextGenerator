using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreDbContextGenerator.CLITest.Models
{
    public class Post
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public Blog Blog { get; set; }
    }

    public class AuditEntry
    {
        public int AuditEntryId { get; set; }
        public string Username { get; set; }
        public string Action { get; set; }
    }
}
