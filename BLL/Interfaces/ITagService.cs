using System.Collections.Generic;
using System.Threading.Tasks;
using BLL.DTO;

namespace BLL.Services
{
    public interface ITagService
    {
        Task<IEnumerable<TagDTO>> GetAllTags();
        Task<TagDTO> GetTagById(int id);
        Task<TagDTO> CreateTag(TagDTO tagDto);
        Task<TagDTO> UpdateTag(int id, TagDTO tagDto);
        Task DeleteTag(int id);
    }
}