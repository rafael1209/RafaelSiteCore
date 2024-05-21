using RafaelSiteCore.DataWrapper.Authorize;
using RafaelSiteCore.Model.User;

namespace RafaelSiteCore.Services.Auth
{
        public class DiscordAuthLogic
        {
                private AuthorizeDbContext _mongoDbContext;

                public DiscordAuthLogic(AuthorizeDbContext mongoDbContext)
                {
                        _mongoDbContext = mongoDbContext;
                }

                public User ReturnUserData(User user)
                {
                        if (_mongoDbContext.IsUserExist(user.DiscordId))
                                return _mongoDbContext.GetUserByIdDiscord(user.DiscordId);

                        return _mongoDbContext.AddAndReturnUser(user);
                }
        }
}
