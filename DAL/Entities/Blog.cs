using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities
{
    public class Blog
    {
        public int BlogId { get; set; }
        public string Name { get; set; }

        [ForeignKey("User")]
        public string OwnerId { get; set; }
        public User Owner { get; set; }
        public ICollection<Article> Articles { get; set; }
    }
}