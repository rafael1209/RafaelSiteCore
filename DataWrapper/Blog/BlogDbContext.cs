using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using RafaelSiteCore.DataWrapper.Authorize;
using RafaelSiteCore.Model.Blog;
using RafaelSiteCore.Model.Users;
using RafaelSiteCore.Services.Blog;
using System.Security.Principal;

namespace RafaelSiteCore.DataWrapper.Blog
{
        public class BlogDbContext
        {
                private readonly MongoClient _mongoClient;

                private readonly IMongoDatabase _mongoDatabase;

                private IMongoCollection<Post> _blogCollection;

                private IMongoCollection<User> _userCollection;

                private const string ConstPostsCollection = "Posts";

                private const string ConstUsersCollection = "Users";

                public BlogDbContext(string connectionString, string databaseName)
                {
                        this._mongoClient = new MongoClient(connectionString);

                        this._mongoDatabase = _mongoClient.GetDatabase(databaseName);

                        this._blogCollection = _mongoDatabase.GetCollection<Post>(ConstPostsCollection);

                        this._userCollection = _mongoDatabase.GetCollection<User>(ConstUsersCollection);
                }

                public List<PostSummary> GetPosts()
                {
                        var projection = Builders<Post>.Projection.Expression(p => new
                        {
                                PostId = p.Id.ToString(),
                                AuthorDiscordId = p.Author.DiscordId,
                                Data = new Content { Title = p.Title, Body = p.Body, Imgurl = p.ImgUrl },
                                Likes = p.Likes.Count,
                                CreatedAtUtc = p.CreatedDateUtc
                        });

                        var allPostsIntermediate = _blogCollection.Find(FilterDefinition<Post>.Empty)
                            .Project(projection)
                            .ToList();

                        var allPosts = allPostsIntermediate.Select(p => new PostSummary
                        {
                                PostId = p.PostId,
                                Author = GetAccountByDiscordID(p.AuthorDiscordId),
                                Data = p.Data,
                                Likes = p.Likes,
                                CretaedAtUtc = p.CreatedAtUtc
                        }).ToList();

                        return allPosts;
                }

                public Account GetAccountByDiscordID(ulong discordID)
                {
                        var user = GetUserByIdDiscord(discordID);

                        return new Account()
                        {
                                DiscordId = discordID,
                                AvatarUrl = user.AvatarUrl,
                                Name = user.Name,
                        };
                }

                internal User GetUserByIdDiscord(ulong idDiscord)
                {
                        var filter = Builders<User>.Filter.Eq(u => u.DiscordId, idDiscord);

                        return _userCollection.Find(filter).FirstOrDefault();
                }

                //public void SavePost(string title, string body, string ImgUrl, User user)
                //{
                //        Post post = new Post();

                //        post.Title = title;
                //        post.body = body;
                //        post.ImgUrl = ImgUrl;
                //        post.Author = new Account() { AvatarUrl = user.AvatarUrl, Name = user.Name, DiscordId = user.DiscordId };
                //        post.CreatedDateUtc = DateTime.UtcNow;

                //        _blogCollection.InsertOne(post);
                //}

                public void LikePost(User user, ObjectId postId)
                {
                        Account account = new Account()
                        {
                                AvatarUrl = user.AvatarUrl,
                                Name = user.Name,
                                DiscordId = user.DiscordId,
                        };

                        var filter = Builders<Post>.Filter.Eq(p => p.Id, postId);
                        var update = Builders<Post>.Update.AddToSet(p => p.Likes, account);

                        _blogCollection.UpdateOne(filter, update);
                }

                public void UnlikePost(User user, ObjectId postId)
                {
                        Account account = new Account()
                        {
                                AvatarUrl = user.AvatarUrl,
                                Name = user.Name,
                                DiscordId = user.DiscordId,
                        };

                        var filter = Builders<Post>.Filter.Eq(p => p.Id, postId);
                        var update = Builders<Post>.Update.Pull(p => p.Likes, account);

                        _blogCollection.UpdateOne(filter, update);
                }
        }
}
