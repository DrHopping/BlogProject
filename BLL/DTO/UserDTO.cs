using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BLL.DTO
{
    public class UserDTO
    {
        public string Id { get; set; }
        public string Username { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
        public string AvatarUrl { get; set; }
        public string Info { get; set; }
        public string Role { get; set; }
        public IEnumerable<BlogDTO> Blogs { get; set; }
        public IEnumerable<CommentDTO> Comments { get; set; }
    }
}