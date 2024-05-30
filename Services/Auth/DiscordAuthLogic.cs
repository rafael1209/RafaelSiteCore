using MongoDB.Bson;
using RafaelSiteCore.DataWrapper.Authorize;
using RafaelSiteCore.Model.Users;

namespace RafaelSiteCore.Services.Auth
{
        public class DiscordAuthLogic
        {
                private AuthorizeDbContext _mongoDbContext;

                public DiscordAuthLogic(AuthorizeDbContext mongoDbContext)
                {
                        _mongoDbContext = mongoDbContext;
                }

                public User ReturnUserData(User user, string avatarHash)
                {
                        if (_mongoDbContext.IsUserExist(user.DiscordId))
                                user = _mongoDbContext.GetUserByIdDiscord(user.DiscordId);
                        else
                                user = _mongoDbContext.AddAndReturnUser(user);

                        if (user.AvatarUrl != avatarHash) 
                                _mongoDbContext.UpdateUserAvatarHash(user.Id, avatarHash);

                        user.AvatarUrl = avatarHash;

                        return user;
                }

                public User IsUserExist(ObjectId AuthToken)
                {
                        return _mongoDbContext.GetAuthenticatedUser(AuthToken);
                }
        }
}
