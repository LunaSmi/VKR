using VKR.API.Models.Attach;
using VKR.API.Models.User;
using VKR.DAL.Entities;

namespace VKR.API.Services
{
    public class LinkGeneratorService
    {
        public Func<PostContent, string?>? LinkContentGenerator;
        public Func<User, string?>? LinkAvatarGenerator;
        public Func<User, string?>? LinkAvatarForCommentGenerator;

        public void FixAvatar(User s, UserAvatarModel d)
        {
            d.AvatarLink = s.Avatar == null ? null : LinkAvatarGenerator?.Invoke(s);
        }
        public void FixContent(PostContent s, AttachModelWithLink d)
        {
            d.ContentLink = LinkContentGenerator?.Invoke(s);
        }
        public void FixAvatar(User s, UserAvatarForCommentModel d)
        {
            d.AvatarLink = s.Avatar == null ? null : LinkAvatarForCommentGenerator?.Invoke(s);
        }

    }
}
