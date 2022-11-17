using AutoMapper;
using VKR.API.Models.Attach;
using VKR.API.Models.Post;
using VKR.API.Models.User;
using VKR.Common;

namespace VKR.API
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

            CreateMap<DAL.Entities.User, UserAvatarModel>();

            CreateMap<VKR.DAL.Entities.Avatar, AttachModel>();

            CreateMap<VKR.DAL.Entities.PostContent, AttachModel>();

            CreateMap<DAL.Entities.PostContent, AttachModelWithLink>();

            CreateMap<MetadataLinkModel, DAL.Entities.PostContent>();

            CreateMap<MetadataModel, MetadataLinkModel>();





            CreateMap<CreatePostRequest, CreatePostModel>()
                .ForMember(d => d.Id, m => m.MapFrom(s => Guid.NewGuid()));


            CreateMap<CreatePostModel, DAL.Entities.Post>()
                .ForMember(d=>d.OwnerId,m=>m.MapFrom(s=>s.AuthorId))
                .ForMember(d => d.Contents, m => m.MapFrom(s => s.Contents))
                .ForMember(d => d.Created, m => m.MapFrom(s => DateTime.UtcNow));


            //-------------------

            //CreateMap<Models.Attach.MetadataModel, DAL.Entities.PostContent>();







        }
    }
}
