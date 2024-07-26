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

                public List<PostViewModel> GetPosts()
                {
                        var posts = _blogCollection.Find(post => true).ToList();

                        var postViewModels = posts.Select(post => new PostViewModel
                        {
                                Id = post.Id.ToString(),
                                Text = post.Text,
                                Imgurl = post.Imgurl,
                                CreatedAtUtc = post.CretaedAtUtc,
                                UpdatedAtUtc = post.UpdatedAtUtc,
                                Account = post.Account,
                                Comments = post.Comments,
                                Likes = post.Likes.Count(),
                        }).ToList();

                        return postViewModels;
                }


                public Account GetAccountByDiscordID(ulong discordID)
                {
                        var user = GetUserByIdDiscord(discordID);

                        return new Account()
                        {
                                SearchToken = user.Id.ToString(),
                                AvatarUrl = user.AvatarUrl,
                                Username = user.Name,
                        };
                }

                internal User GetUserByIdDiscord(ulong idDiscord)
                {
                        var filter = Builders<User>.Filter.Eq(u => u.DiscordId, idDiscord);

                        return _userCollection.Find(filter).FirstOrDefault();
                }

                public void SavePost(string text, string imgUrl, User user)
                {
                        Post post = new Post();

                        post.Text = text;
                        post.Imgurl = imgUrl;
                        post.CretaedAtUtc = DateTime.UtcNow;
                        post.UpdatedAtUtc = DateTime.UtcNow;
                        post.Account = new Account() { SearchToken = user.Id.ToString(), Username = user.Name, AvatarUrl = user.AvatarUrl };

                        _blogCollection.InsertOne(post);
                }

                public void LikePost(User user, ObjectId postId)
                {
                        Account account = new Account()
                        {
                                SearchToken= user.Id.ToString(),
                                AvatarUrl = user.AvatarUrl,
                                Username = user.Name
                        };

                        var filter = Builders<Post>.Filter.Eq(p => p.Id, postId);
                        var update = Builders<Post>.Update.AddToSet(p => p.Likes, account);

                        _blogCollection.UpdateOne(filter, update);
                }

                public void UnlikePost(User user, ObjectId postId)
                {
                        Account account = new Account()
                        {
                                SearchToken = user.Id.ToString(),
                                AvatarUrl = user.AvatarUrl,
                                Username = user.Name
                        };

                        var filter = Builders<Post>.Filter.Eq(p => p.Id, postId);
                        var update = Builders<Post>.Update.Pull(p => p.Likes, account);

                        _blogCollection.UpdateOne(filter, update);
                }
        }
}
