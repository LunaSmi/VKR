using AutoMapper;
using VKR.API.Models.Attach;
using VKR.API.Models.User;
using VKR.API.Services;
using VKR.DAL.Entities;

namespace VKR.API.Mapper.MapperActions
{
    public class PostContentMapperAction : IMappingAction<DAL.Entities.PostContent, AttachModelWithLink>
    {
        private LinkGeneratorService _links;
        public PostContentMapperAction(LinkGeneratorService links)
        {
            _links = links;
        }

        public void Process(PostContent source, AttachModelWithLink destination, ResolutionContext context)
        {
            _links.FixContent(source,destination);
        }
    }
}
