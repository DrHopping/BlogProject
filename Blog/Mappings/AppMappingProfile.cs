using BLL.DTO;
using BLL.Mappings;
using Blog.Models;

namespace Blog.Mappings
{
    public class AppMappingProfile : MappingProfile
    {
        public AppMappingProfile()
        {
            CreateMap<RegisterModel, UserDTO>()
                .ForMember(dto => dto.UserName, opt => opt.MapFrom(model => model.Username));
            CreateMap<BlogCreateModel, BlogDTO>();
        }
    }
}