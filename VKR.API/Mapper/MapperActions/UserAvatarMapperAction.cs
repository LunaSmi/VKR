using AutoMapper;
using VKR.API.Models.User;
using VKR.API.Services;
using VKR.DAL.Entities;

namespace VKR.API.Mapper.MapperActions
{
    public class UserAvatarMapperAction : IMappingAction<DAL.Entities.User, UserAvatarModel>
    {
        private LinkGeneratorService _links;
        public UserAvatarMapperAction(LinkGeneratorService links)
        {
            _links = links;
        }

        public void Process(User source, UserAvatarModel destination, ResolutionContext context)
        {
            _links.FixAvatar(source, destination);
        }
    }
}
