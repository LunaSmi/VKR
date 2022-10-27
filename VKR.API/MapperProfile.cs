using AutoMapper;
using VKR.Common;

namespace VKR.API
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Models.CreateUserModel, VKR.DAL.Entities.User>()
                .ForMember(d => d.Id, m => m.MapFrom(s => Guid.NewGuid()))
                .ForMember(d=>d.PasswordHash,m=>m.MapFrom(s=>HashHelper.GetHash(s.Password)))
                .ForMember(d=>d.BirthDate,m=>m.MapFrom(s=>s.BirthDate.UtcDateTime));

            CreateMap<VKR.DAL.Entities.User, Models.UserModel>();
        }
    }
}
