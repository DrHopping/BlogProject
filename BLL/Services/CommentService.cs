using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BLL.DTO;
using BLL.Exceptions;
using BLL.Interfaces;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace BLL.Services
{
    public class CommentService : ICommentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtFactory _jwtFactory;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public CommentService(IUnitOfWork unitOfWork, IJwtFactory jwtFactory, UserManager<User> userManager, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _jwtFactory = jwtFactory;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<CommentDTO> AddComment(CommentDTO comment, string token)
        {
            if (comment == null) throw new ArgumentNullException(nameof(comment));
            if (comment.Content == null) throw new ArgumentNullException(nameof(comment.Content));
            if (comment.ArticleId == null) throw new ArgumentNullException(nameof(comment.ArticleId));
            if (token == null) throw new ArgumentNullException(nameof(token));

            var userId = _jwtFactory.GetUserIdClaim(token);
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) throw new EntityNotFoundException(nameof(user), userId);

            var commentEntity = _mapper.Map<CommentDTO, Comment>(comment);
            commentEntity.UserId = userId;

            commentEntity = await _unitOfWork.CommentRepository.InsertAndSaveAsync(commentEntity);

            var result = _mapper.Map<Comment, CommentDTO>(commentEntity);
            result.CreatorUsername = user.UserName;
            return result;
        }

        public async Task DeleteComment(int id, string token)
        {
            var comment = await _unitOfWork.CommentRepository.GetByIdAsync(id);
            if (comment == null) throw new EntityNotFoundException(nameof(comment), id);

            var userId = _jwtFactory.GetUserIdClaim(token);
            if (comment.UserId != userId)
            {
                var userRole = _jwtFactory.GetUserRoleClaim(token);
                if (!userRole.Equals("Moderator")) throw new NotEnoughRightsException();
            }
            await _unitOfWork.CommentRepository.DeleteAndSaveAsync(comment);
        }

        public async Task UpdateComment(int id, CommentDTO commentDto, string token)
        {
            if (commentDto == null) throw new ArgumentNullException(nameof(commentDto));
            if (token == null) throw new ArgumentNullException(nameof(token));

            var comment = await _unitOfWork.CommentRepository.GetByIdAsync(id);
            if (comment == null) throw new EntityNotFoundException(nameof(comment), id);

            var userId = _jwtFactory.GetUserIdClaim(token);
            if (!comment.UserId.Equals(userId)) throw new NotEnoughRightsException();

            if (commentDto.Content != null) comment.Content = commentDto.Content;
            comment.LastUpdated = DateTime.Now;
            await _unitOfWork.CommentRepository.UpdateAndSaveAsync(comment);
        }

        public async Task<CommentDTO> GetCommentById(int id)
        {
            var comment = await _unitOfWork.CommentRepository.GetByIdAsync(id);
            if (comment == null) throw new EntityNotFoundException(nameof(comment), id);
            var result = _mapper.Map<Comment, CommentDTO>(comment);
            return result;
        }

        public async Task<IEnumerable<CommentDTO>> GetAllComments()
        {
            var comments = await _unitOfWork.CommentRepository.GetAllAsync();
            var result = _mapper.Map<IEnumerable<Comment>, IEnumerable<CommentDTO>>(comments);
            return result;
        }

        public async Task<IEnumerable<CommentDTO>> GetAllCommentsByUserId(int id)
        {
            var userEntity = await _userManager.FindByIdAsync(id.ToString());
            if (userEntity == null) throw new EntityNotFoundException(nameof(userEntity), id);
            var comments = await _unitOfWork.CommentRepository.GetAllAsync(c => c.UserId == id);
            return _mapper.Map<IEnumerable<Comment>, IEnumerable<CommentDTO>>(comments);
        }
    }
}