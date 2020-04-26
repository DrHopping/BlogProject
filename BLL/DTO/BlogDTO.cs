using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.DTO
{
    public class BlogDTO
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public int OwnerId { get; set; }
        public string OwnerUsername { get; set; }
        public IEnumerable<ArticleDTO> Articles { get; set; }
    }
}
