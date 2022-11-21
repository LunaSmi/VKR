using AutoMapper;
using VKR.API.Mapper.MapperActions;
using VKR.API.Models.Attach;
using VKR.API.Models.Post;
using VKR.API.Models.User;
using VKR.Common;
using VKR.DAL.Entities;

namespace VKR.API.Mapper
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<CreateUserModel, VKR.DAL.Entities.User>()
                .ForMember(d => d.Id, m => m.MapFrom(s => Guid.NewGuid()))
                .ForMember(d=>d.PasswordHash,m=>m.MapFrom(s=>HashHelper.GetHash(s.Password)))
                .ForMember(d=>d.BirthDate,m=>m.MapFrom(s=>s.BirthDate.UtcDateTime));

            CreateMap<VKR.DAL.Entities.User, UserModel>();

            CreateMap<DAL.Entities.User, UserAvatarModel>()
                .ForMember(d => d.PostsCount, m => m.MapFrom(s => s.Posts!.Count))
                .AfterMap<UserAvatarMapperAction>();

            CreateMap<VKR.DAL.Entities.Avatar, AttachModel>();

            CreateMap<VKR.DAL.Entities.Post, PostModel>()
                .ForMember(d => d.Author, m => m.MapFrom(d => d.Owner))
                .ForMember(d => d.LikesCount, m => m.MapFrom(s => s.PostLikes!.Count))
                .ForMember(d => d.CommentsCount, m => m.MapFrom(s => s.Comments!.Count))
                ;

            CreateMap<VKR.DAL.Entities.PostContent, AttachModel>();

            CreateMap<DAL.Entities.PostContent, AttachModelWithLink>()
                .AfterMap<PostContentMapperAction>();

            CreateMap<MetadataLinkModel, DAL.Entities.PostContent>();

            CreateMap<MetadataModel, MetadataLinkModel>();

            CreateMap<CreatePostRequest, CreatePostModel>()
                .ForMember(d => d.Id, m => m.MapFrom(s => Guid.NewGuid()));

            CreateMap<CreatePostModel, DAL.Entities.Post>()
                .ForMember(d=>d.OwnerId,m=>m.MapFrom(s=>s.AuthorId))
                .ForMember(d => d.Contents, m => m.MapFrom(s => s.Contents))
                .ForMember(d => d.Created, m => m.MapFrom(s => DateTime.UtcNow));

            CreateMap<CreateCommentRequest, VKR.DAL.Entities.Comment>()
                .ForMember(d => d.Id, m => m.MapFrom(s => Guid.NewGuid()))
                .ForMember(d => d.PublicationDate, m => m.MapFrom(s => DateTime.UtcNow));


            CreateMap<DAL.Entities.User, UserAvatarForCommentModel>()
                .ForMember(d=>d.AuthorId,m => m.MapFrom(s => s.Id))
                .ForMember(d => d.AuthorName, m => m.MapFrom(s => s.Name))
                .AfterMap<UserAvatarForCommentMapperAction>();


            CreateMap<VKR.DAL.Entities.Comment, CommentModel>()
            .ForMember(d => d.Author, m => m.MapFrom(d => d.Author));


        }




    }

    //public class CommentConverter : IValueConverter<ICollection<Comment>, List<CommentModel>>
    //{

    //    public List<CommentModel> Convert(ICollection<Comment> sourceMember, ResolutionContext context)
    //    {
    //        var result = new List<CommentModel>();
            
    //        foreach (var item in sourceMember)
    //        {
    //            var commentModel = new CommentModel();
    //            commentModel = context.Mapper.Map<Comment, CommentModel>(item);
    //            commentModel.Author = context.Mapper.Map<UserAvatarForCommentModel>(item.Author);  

    //           result.Add(commentModel);
    //        }
    //        return result;
    //    }
    //}
}
