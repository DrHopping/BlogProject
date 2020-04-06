﻿using AutoMapper;
using BLL.DTO;
using DAL.Entities;

namespace BLL.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //Entity -> DTO
            CreateMap<Article, ArticleDTO>()
                .ForMember(dto => dto.Tags, opt => opt.MapFrom(ent => ent.Tags))
                .ForMember(dto => dto.Comments, opt => opt.MapFrom(ent => ent.Comments))
                .ForMember(dto => dto.AuthorId, opt => opt.MapFrom(ent => ent.Blog.OwnerId))
                .ForMember(dto => dto.AuthorUsername, opt => opt.MapFrom(ent => ent.Blog.Owner.UserName));

            CreateMap<Blog, BlogDTO>()
                .ForMember(dto => dto.Articles, opt => opt.MapFrom(ent => ent.Articles))
                .ForMember(dto => dto.OwnerUsername, opt => opt.MapFrom(ent => ent.Owner.UserName));

            CreateMap<Comment, CommentDTO>()
                .ForMember(dto => dto.CreatorId, opt => opt.MapFrom(ent => ent.UserId))
                .ForMember(dto => dto.CreatorUsername, opt => opt.MapFrom(ent => ent.User.UserName));

            CreateMap<Tag, TagDTO>();

            CreateMap<User, UserDTO>()
                .ForMember(dto => dto.UserId, opt => opt.MapFrom(ent => ent.Id))
                .ForMember(dto => dto.UserName, opt => opt.MapFrom(ent => ent.UserName))
                .ForMember(dto => dto.Blogs, opt => opt.MapFrom(ent => ent.Blogs))
                .ForMember(dto => dto.Comments, opt => opt.MapFrom(ent => ent.Comments))
                .ForMember(dto => dto.Email, opt => opt.MapFrom(ent => ent.Email))
                .ForMember(dto => dto.Password, opt => opt.Ignore());

            //DTO -> Entity
            CreateMap<ArticleDTO, Article>()
                .ForMember(ent => ent.Content, opt => opt.MapFrom(dto => dto.Content))
                .ForMember(ent => ent.Title, opt => opt.MapFrom(dto => dto.Title))
                .ForMember(ent => ent.BlogId, opt => opt.MapFrom(dto => dto.BlogId))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<BlogDTO, Blog>()
                .ForMember(ent => ent.Name, opt => opt.MapFrom(dto => dto.Name))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<CommentDTO, Comment>()
                .ForMember(ent => ent.Content, opt => opt.MapFrom(dto => dto.Content))
                .ForMember(ent => ent.ArticleId, opt => opt.MapFrom(dto => dto.ArticleId))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<TagDTO, Tag>()
                .ForMember(ent => ent.Name, opt => opt.MapFrom(dto => dto.Name))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<UserDTO, User>()
                .ForMember(ent => ent.UserName, opt => opt.MapFrom(dto => dto.UserName))
                .ForMember(ent => ent.Email, opt => opt.MapFrom(dto => dto.Email))
                .ForAllOtherMembers(opt => opt.Ignore());















        }
    }
}