using System;
using System.Collections.Generic;
using System.Text;
using BLL.DTO;
using DAL.Entities;

namespace BLL.Mappers
{
    public class BlogMapper : BaseMapper<BlogDTO, Blog>
    {
        public override BlogDTO Map(Blog element)
        {
            return new BlogDTO
            {
                BlogId = element.BlogId,
                Name = element.Name,
                OwnerId = element.OwnerId,
            };
        }

        public override Blog Map(BlogDTO element)
        {
            return new Blog
            {
                Name = element.Name
            };
        }
    }
}
