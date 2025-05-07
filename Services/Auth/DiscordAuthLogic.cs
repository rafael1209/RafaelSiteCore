using MongoDB.Bson;
using RafaelSiteCore.DataWrapper.Authorize;
using RafaelSiteCore.Helpers;
using RafaelSiteCore.Model.Users;

namespace RafaelSiteCore.Services.Auth;

public class DiscordAuthLogic(AuthorizeDbContext mongoDbContext)
{
    public User ReturnUserData(User user, string avatarHash)
    {
        user = mongoDbContext.IsUserExist(user.DiscordId)
            ? mongoDbContext.GetUserByIdDiscord(user.DiscordId)
            : mongoDbContext.AddAndReturnUser(user, GenerateAuthToken(user.DiscordId.ToString()));

        if (user.AvatarUrl != avatarHash)
            mongoDbContext.UpdateUserAvatarHash(user.Id, avatarHash);

        user.AvatarUrl = avatarHash;

        return user;
    }

    public User GetUser(string AuthToken)
    {
        return mongoDbContext.GetAuthenticatedUser(AuthToken);
    }

    public string GenerateAuthToken(string id)
    {
        var salt = StringHelpers.GenerateRandomSalt();

        var idSalted = id + salt;

        var authToken = StringHelpers.GenerateHash(idSalted);

        return authToken;
    }
}