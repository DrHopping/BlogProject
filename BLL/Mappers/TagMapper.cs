using BLL.DTO;
using DAL.Entities;

namespace BLL.Mappers
{
    public class TagMapper : BaseMapper<Tag, TagDTO>
    {
        public override Tag Map(TagDTO element)
        {
            return new Tag
            {
                Name = element.Name
            };
        }

        public override TagDTO Map(Tag element)
        {
            return new TagDTO
            {
                TagId = element.TagId,
                Name = element.Name
            };
        }
    }
}