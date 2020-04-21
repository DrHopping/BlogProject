using System;
using System.Collections.Generic;
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
    }
}