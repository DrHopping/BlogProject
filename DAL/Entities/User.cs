using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Identity;

namespace DAL.Entities
{
    public class User : IdentityUser
    {
        public IEnumerable<Comment> Comments { get; set; }
        public IEnumerable<Blog> Blogs { get; set; }
    }
}