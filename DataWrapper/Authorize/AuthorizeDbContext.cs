﻿using MongoDB.Bson;
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

                internal User GetAuthenticatedUser(string authToken)
                {
                        var filter = Builders<User>.Filter.Eq(u => u.AuthToken, authToken);
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

                internal User AddAndReturnUser(User user,string authToken)
                {
                        user.Balance = 0;

                        user.LastLoginUtc = DateTime.UtcNow;

                        user.CreatedDateUtc = DateTime.UtcNow;

                        user.AuthToken = authToken;

                        _usersCollection.InsertOne(user);

                        return user;
                }

                internal void UpdateUserAvatarHash(ObjectId searchToken, string newAvatarHash)
                {
                        var filter = Builders<User>.Filter.Eq(u => u.Id, searchToken);

                        var update = Builders<User>.Update.Set(u => u.AvatarUrl, newAvatarHash);

                        _usersCollection.UpdateOne(filter, update);
                }

                internal Account GetAccountByIdDiscord(ulong idDiscord)
                {
                        Account account = new Account();

                        var filter = Builders<User>.Filter.Eq(u => u.DiscordId, idDiscord);

                        var user = _usersCollection.Find(filter).FirstOrDefault();

                        account.SearchToken = user.Id.ToString();

                        account.Username = user.Name;

                        account.AvatarUrl = $"{user.AvatarUrl}";

                        account.IsVerified = user.IsVerified;

                        return account;
                }
        }
}
