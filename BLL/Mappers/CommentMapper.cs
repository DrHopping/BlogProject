using System;
using System.Collections.Generic;
using System.Text;
using BLL.DTO;
using DAL.Entities;

namespace BLL.Mappers
{
    public class CommentMapper : BaseMapper<Comment, CommentDTO>
    {
        public override Comment Map(CommentDTO element)
        {
            return new Comment
            {
                Content = element.Content,
                ArticleId = element.ArticleId.GetValueOrDefault()
            };
        }

        public override CommentDTO Map(Comment element)
        {
            return new CommentDTO
            {
                CommentId = element.CommentId,
                Content = element.Content,
                ArticleId = element.ArticleId,
                CreatorId = element.UserId,
                CreatorUsername = element.User.UserName
            };
        }
    }
}
