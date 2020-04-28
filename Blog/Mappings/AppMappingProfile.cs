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
                .ForMember(dto => dto.Username, opt => opt.MapFrom(model => model.Username));
            CreateMap<BlogCreateModel, BlogDTO>();
            CreateMap<TagCreateModel, TagDTO>();
            CreateMap<ArticleCreateModel, ArticleDTO>();
            CreateMap<ArticleUpdateModel, ArticleDTO>();
            CreateMap<CommentCreateModel, CommentDTO>();
            CreateMap<CommentUpdateModel, CommentDTO>();
            CreateMap<AuthenticateModel, UserDTO>();
            CreateMap<UserUpdateModel, UserDTO>();

        }
    }
}