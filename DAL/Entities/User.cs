using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Identity;

namespace DAL.Entities
{
    public class User : IdentityUser
    {
        public ICollection<Comment> Comments { get; set; }
        public ICollection<Blog> Blog { get; set; }
    }
}