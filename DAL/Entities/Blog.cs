using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using DAL.Entities.Base;

namespace DAL.Entities
{
    public class Blog : EntityBase<int>
    {
        public string Name { get; set; }

        [ForeignKey("User")]
        public string OwnerId { get; set; }
        public User Owner { get; set; }
        public IEnumerable<Article> Articles { get; set; }
    }
}