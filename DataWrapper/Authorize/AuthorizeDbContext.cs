using MongoDB.Bson;
using MongoDB.Driver;
using RafaelSiteCore.Model.User;

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

                        this._usersCollection = _mongoDatabase.GetCollection<User>(AuthorizeDbContext.ConstUsersCollection);
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
        }
}
