using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BLL.DTO;
using BLL.Exceptions;
using BLL.Interfaces;
using BLL.Mappers;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace BLL.Services
{
    public class CommentService
    {
        private IUnitOfWork _unitOfWork;
        private IJwtFactory _jwtFactory;
        private UserManager<User> _userManager;
        private CommentMapper _commentMapper;

        public CommentService(IUnitOfWork unitOfWork, IJwtFactory jwtFactory, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _jwtFactory = jwtFactory;
            _userManager = userManager;
        }

        private CommentMapper CommentMapper => _commentMapper ??= new CommentMapper();

        public async Task<CommentDTO> AddComment(CommentDTO comment, string token)
        {
            if (comment == null) throw new ArgumentNullException(nameof(comment));
            if (comment.Content == null) throw new ArgumentNullException(nameof(comment.Content));
            if (comment.ArticleId == null) throw new ArgumentNullException(nameof(comment.ArticleId));
            if (token == null) throw new ArgumentNullException(nameof(token));

            var userId = _jwtFactory.GetUserIdClaim(token);
            if (userId == null) throw new ArgumentNullException(nameof(userId));
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) throw new ArgumentNullException(nameof(user));

            var commentEntity = CommentMapper.Map(comment);
            commentEntity.UserId = userId;

            _unitOfWork.CommentRepository.Insert(commentEntity);
            await _unitOfWork.SaveAsync();
            var result = CommentMapper.Map(commentEntity);
            result.CreatorUsername = user.UserName;
            return result;
        }

        public async Task DeleteComment(int id, string token)
        {
            var entity = _unitOfWork.CommentRepository.GetById(id);
            if (entity == null) throw new ArgumentNullException(nameof(entity), $"Cant find comment with id {id}");

            var userId = _jwtFactory.GetUserIdClaim(token);
            if (userId == null) throw new ArgumentNullException(nameof(userId));
            if (entity.UserId != userId)
            {
                var userRole = _jwtFactory.GetUserRoleClaim(token);
                if (!userRole.Equals("Moderator")) throw new NotEnoughRightsException();
            }
            _unitOfWork.CommentRepository.Delete(entity);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateComment(int id, CommentDTO comment, string token)
        {
            if (comment == null) throw new ArgumentNullException(nameof(comment));
            if (token == null) throw new ArgumentNullException(nameof(token));

            var entity = _unitOfWork.CommentRepository.GetById(id);
            if (entity == null) throw new ArgumentNullException(nameof(entity), $"Cant find comment with id {id}");
            var userId = _jwtFactory.GetUserIdClaim(token);
            if (userId == null) throw new ArgumentNullException(nameof(userId));
            if (!entity.UserId.Equals(userId)) throw new NotEnoughRightsException();

            if (comment.Content != null) entity.Content = comment.Content;
            _unitOfWork.CommentRepository.Update(entity);
            await _unitOfWork.SaveAsync();
        }

        public async Task<CommentDTO> GetCommentById(int id)
        {
            var comment = await _unitOfWork.CommentRepository.GetByIdAsync(id);
            if (comment == null) throw new ArgumentNullException(nameof(comment), $"Cant find comment with id {id}");
            var result = CommentMapper.Map(comment);
            return result;
        }

        public async Task<IEnumerable<CommentDTO>> GetAllComments()
        {
            var comments = await _unitOfWork.CommentRepository.Get();
            if (comments == null) throw new ArgumentNullException(nameof(comments));
            var result = CommentMapper.Map(comments);
            return result;
        }
    }
}