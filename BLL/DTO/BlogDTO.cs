using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.DTO
{
    public class BlogDTO
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string OwnerId { get; set; }
        public string OwnerUsername { get; set; }
        public ICollection<ArticleDTO> Articles { get; set; }
    }
}
