using AutoMapper;
using VKR.API.Models.Attach;
using VKR.API.Models.Post;
using VKR.Common;

namespace VKR.API
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Models.User.CreateUserModel, VKR.DAL.Entities.User>()
                .ForMember(d => d.Id, m => m.MapFrom(s => Guid.NewGuid()))
                .ForMember(d=>d.PasswordHash,m=>m.MapFrom(s=>HashHelper.GetHash(s.Password)))
                .ForMember(d=>d.BirthDate,m=>m.MapFrom(s=>s.BirthDate.UtcDateTime));

            CreateMap<VKR.DAL.Entities.User, Models.User.UserModel>();

            CreateMap<VKR.DAL.Entities.Avatar, Models.Attach.AttachModel>();

            CreateMap<VKR.DAL.Entities.Photo, Models.Attach.AttachModel>();

            CreateMap<Models.Post.CreatePostModel, VKR.DAL.Entities.Post>()
                .ForMember(d => d.Id, m => m.MapFrom(s => Guid.NewGuid()));

            CreateMap<Models.Attach.MetaDataModel, DAL.Entities.Photo>();

            CreateMap<Models.Attach.MetadataWithPath, DAL.Entities.Photo>();

            CreateMap<CreatePostModel, DAL.Entities.Post>()
                .ForMember(d => d.Photos, m => m.MapFrom(s => s.Contents))
                .ForMember(d => d.Created, m => m.MapFrom(s => DateTime.UtcNow));
        }
    }
}
