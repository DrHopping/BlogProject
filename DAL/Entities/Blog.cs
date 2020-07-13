using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using DAL.Entities.Base;

namespace DAL.Entities
{
    public class Blog : EntityBase<int>
    {
        public string Name { get; set; }
        public string Description { get; set; }

        [ForeignKey("User")]
        public int OwnerId { get; set; }
        public User Owner { get; set; }
        public IEnumerable<Article> Articles { get; set; }
    }
}