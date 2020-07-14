using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BLL.DTO;
using BLL.Exceptions;
using BLL.Exceptions.Base;
using DAL.Entities;
using DAL.Interfaces;

namespace BLL.Services
{
    public class TagService : ITagService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public TagService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        public async Task<IEnumerable<TagDTO>> GetAllTags()
        {
            return _mapper.Map<IEnumerable<Tag>, IEnumerable<TagDTO>>(await _unitOfWork.TagRepository.GetAllAsync());
        }

        public async Task<TagDTO> GetTagById(int id)
        {
            var tag = await _unitOfWork.TagRepository.GetByIdAsync(id);
            if (tag == null) throw new EntityNotFoundException(nameof(Tag), id);
            return _mapper.Map<Tag, TagDTO>(tag);
        }

        public async Task<TagDTO> CreateTag(TagDTO tagDto)
        {
            var tag = _mapper.Map<Tag>(tagDto);
            var result = await _unitOfWork.TagRepository.InsertAndSaveAsync(tag);
            return _mapper.Map<TagDTO>(result);
        }

        public async Task<TagDTO> UpdateTag(int id, TagDTO tagDto)
        {
            var tag = await _unitOfWork.TagRepository.GetByIdAsync(id);
            if (tag == null) throw new EntityNotFoundException(nameof(tag), id);
            tag.Name = tagDto.Name;
            var result = await _unitOfWork.TagRepository.UpdateAndSaveAsync(tag);
            return _mapper.Map<TagDTO>(result);
        }

        public async Task DeleteTag(int id)
        {
            var tag = await _unitOfWork.TagRepository.GetByIdAsync(id);
            if (tag == null) throw new EntityNotFoundException(nameof(tag), id);
            await _unitOfWork.TagRepository.DeleteAndSaveAsync(tag);
        }

        public async Task<IEnumerable<TagDTO>> GetTopTags()
        {
            var topTags = (await _unitOfWork.TagRepository.GetAllAsync("ArticleTags"))
                .OrderByDescending(t => t.ArticleTags.Count());

            return _mapper.Map<IEnumerable<TagDTO>>(topTags);
        }
    }
}