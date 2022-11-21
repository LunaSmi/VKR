using AutoMapper;
using VKR.API.Models.User;
using VKR.API.Services;
using VKR.DAL.Entities;

namespace VKR.API.Mapper.MapperActions
{
    public class UserAvatarForCommentMapperAction : IMappingAction<DAL.Entities.User, UserAvatarForCommentModel>
    {
        private LinkGeneratorService _links;
        public UserAvatarForCommentMapperAction(LinkGeneratorService links)
        {
            _links = links;
        }

        public void Process(User source, UserAvatarForCommentModel destination, ResolutionContext context)
        {
            _links.FixAvatar(source, destination);
        }
    }
}
