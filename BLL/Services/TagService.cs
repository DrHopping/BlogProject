using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BLL.DTO;
using BLL.Mappers;
using DAL.Interfaces;

namespace BLL.Services
{
    public class TagService
    {
        private IUnitOfWork _unitOfWork;
        private TagMapper _tagMapper;

        public TagService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        private TagMapper TagMapper => _tagMapper ??= new TagMapper();

        public IEnumerable<TagDTO> GetAllTags()
        {
            return TagMapper.Map(_unitOfWork.TagRepository.Get());
        }

        public async Task<TagDTO> GetTagById(int id)
        {
            var tag = await _unitOfWork.TagRepository.GetByIdAsync(id);
            if (tag == null) throw new ArgumentNullException(nameof(tag));
            return TagMapper.Map(tag);
        }
    }
}