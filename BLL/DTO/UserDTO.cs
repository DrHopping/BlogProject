using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BLL.DTO
{
    public class UserDTO
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
        public IEnumerable<BlogDTO> Blogs { get; set; }
        public IEnumerable<CommentDTO> Comments { get; set; }
    }
}