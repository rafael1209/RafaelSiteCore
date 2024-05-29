using MongoDB.Bson;
using MongoDB.Driver;
using RafaelSiteCore.Model.Users;

namespace RafaelSiteCore.DataWrapper.Authorize
{
        public class AuthorizeDbContext
        {
                private readonly MongoClient _mongoClient;

                private readonly IMongoDatabase _mongoDatabase;

                private IMongoCollection<User> _usersCollection;

                private const string ConstUsersCollection = "Users";


                public AuthorizeDbContext(string connectionString, string databaseName)
                {
                        this._mongoClient = new MongoClient(connectionString);

                        this._mongoDatabase = _mongoClient.GetDatabase(databaseName);

                        this._usersCollection = _mongoDatabase.GetCollection<User>(ConstUsersCollection);
                }

                internal User GetAuthenticatedUser(ObjectId authToken)
                {
                        var filter = Builders<User>.Filter.Eq(u => u.Id, authToken);
                        var user = _mongoDatabase.GetCollection<User>(ConstUsersCollection).Find(filter).FirstOrDefault();
                        return user;
                }

                internal bool IsUserExist(ulong idDiscord)
                {
                        var filter = Builders<User>.Filter.Eq(u => u.DiscordId, idDiscord);

                        return _mongoDatabase.GetCollection<User>(ConstUsersCollection).Find(filter).Any();
                }

                internal User GetUserByIdDiscord(ulong idDiscord)
                {
                                var filter = Builders<User>.Filter.Eq(u => u.DiscordId, idDiscord);

                                return _usersCollection.Find(filter).FirstOrDefault();
                }

                internal User AddAndReturnUser(User user)
                {
                        user.Balance = 0;

                        user.LastLoginUtc = DateTime.UtcNow;

                        user.CreatedDateUtc = DateTime.UtcNow;

                        _usersCollection.InsertOne(user);

                        return user;
                }

                internal void UpdateUserAvatarHash(ObjectId searchToken, string newAvatarHash)
                {
                        var filter = Builders<User>.Filter.Eq(u => u.Id, searchToken);

                        var update = Builders<User>.Update.Set(u => u.AvatarHash, newAvatarHash);

                        _usersCollection.UpdateOne(filter, update);
                }

                internal Account GetAccountByIdDiscord(ulong idDiscord)
                {
                        Account account = new Account();

                        var filter = Builders<User>.Filter.Eq(u => u.DiscordId, idDiscord);

                        var user = _usersCollection.Find(filter).FirstOrDefault();

                        account.DiscordId = user.DiscordId;

                        account.Name = user.Name;

                        account.AvatarUrl = $"https://cdn.discordapp.com/avatars/{user.DiscordId}/{user.AvatarHash}.png";

                        return account;
                }
        }
}
