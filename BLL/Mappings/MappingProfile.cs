using System.Linq;
using AutoMapper;
using BLL.DTO;
using BLL.Security;
using DAL.Entities;

namespace BLL.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //Entity -> DTO
            CreateMap<Article, ArticleDTO>()
                .ForMember(dto => dto.Tags, opt => opt.MapFrom(ent => ent.ArticleTags.Select(at => at.Tag)))
                .ForMember(dto => dto.AuthorId, opt => opt.MapFrom(ent => ent.Blog.OwnerId))
                .ForMember(dto => dto.BlogName, opt => opt.MapFrom(ent => ent.Blog.Name))
                .ForMember(dto => dto.AuthorUsername, opt => opt.MapFrom(ent => ent.Blog.Owner.UserName));

            CreateMap<Blog, BlogDTO>()
                .ForMember(dto => dto.OwnerUsername, opt => opt.MapFrom(ent => ent.Owner.UserName));

            CreateMap<Comment, CommentDTO>()
                .ForMember(dto => dto.CreatorId, opt => opt.MapFrom(ent => ent.UserId))
                .ForMember(dto => dto.CreatorUsername, opt => opt.MapFrom(ent => ent.User.UserName))
                .ForMember(dto => dto.CreatorAvatarUrl, opt => opt.MapFrom(ent => ent.User.AvatarUrl));


            CreateMap<Tag, TagDTO>();

            CreateMap<User, UserDTO>()
                .ForMember(dto => dto.Username, opt => opt.MapFrom(ent => ent.UserName))
                .ForMember(dto => dto.PhoneNumber, opt => opt.MapFrom(ent => Encryption.DecryptData(ent.PhoneNumber, "8ACF8A0C85C4F381CD3B7570F290F8FB843BB916F81E053022E633ADB4045C97")))
                .ForMember(dto => dto.Password, opt => opt.Ignore());

            CreateMap<User, PublicUserInfoDTO>()
                .ForMember(dto => dto.Username, opt => opt.MapFrom(ent => ent.UserName));

            //DTO -> Entity
            CreateMap<ArticleDTO, Article>()
                .ForMember(ent => ent.Content, opt => opt.MapFrom(dto => dto.Content))
                .ForMember(ent => ent.Title, opt => opt.MapFrom(dto => dto.Title))
                .ForMember(ent => ent.BlogId, opt => opt.MapFrom(dto => dto.BlogId))
                .ForMember(ent => ent.ImageUrl, opt => opt.MapFrom(dto => dto.ImageUrl))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<BlogDTO, Blog>()
                .ForMember(ent => ent.Name, opt => opt.MapFrom(dto => dto.Name))
                .ForMember(ent => ent.Description, opt => opt.MapFrom(dto => dto.Description))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<CommentDTO, Comment>()
                .ForMember(ent => ent.Content, opt => opt.MapFrom(dto => dto.Content))
                .ForMember(ent => ent.ArticleId, opt => opt.MapFrom(dto => dto.ArticleId))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<TagDTO, Tag>()
                .ForMember(ent => ent.Name, opt => opt.MapFrom(dto => dto.Name))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<UserDTO, User>()
                .ForMember(ent => ent.UserName, opt => opt.MapFrom(dto => dto.Username))
                .ForMember(ent => ent.Email, opt => opt.MapFrom(dto => dto.Email))
                .ForAllOtherMembers(opt => opt.Ignore());















        }
    }
}