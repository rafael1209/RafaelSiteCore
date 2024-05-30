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

                private const string ConstPostsCollection = "Posts";

                public BlogDbContext(string connectionString, string databaseName)
                {
                        this._mongoClient = new MongoClient(connectionString);

                        this._mongoDatabase = _mongoClient.GetDatabase(databaseName);

                        this._blogCollection = _mongoDatabase.GetCollection<Post>(ConstPostsCollection);
                }

                public List<PostSummary> GetPosts()
                {
                        var projection = Builders<Post>.Projection.Expression(p => new PostSummary
                        {
                                PostId = p.Id.ToString(),
                                Author = p.Author,
                                Title = p.Title,
                                Body = p.body,
                                Imgurl = p.ImgUrl,
                                Likes = p.Likes.Count,
                                CretaedAtUtc = p.CreatedDateUtc
                        }); ;

                        var allPosts = _blogCollection.Find(FilterDefinition<Post>.Empty)
                                .Project(projection)
                                .ToList();

                        return allPosts;
                }

                public void SavePost(string title, string body, string ImgUrl, User user)
                {
                        Post post = new Post();

                        post.Title = title;
                        post.body = body;
                        post.ImgUrl = ImgUrl;
                        post.Author = new Account() { AvatarUrl = user.AvatarUrl, Name = user.Name, DiscordId = user.DiscordId };
                        post.CreatedDateUtc = DateTime.UtcNow;

                        _blogCollection.InsertOne(post);
                }

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
