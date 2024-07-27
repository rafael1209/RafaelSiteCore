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
                        var posts = _blogCollection.Find(post => true)
                                               .SortByDescending(post => post.CretaedAtUtc)
                                               .ToList();

                        var postViewModels = posts.Select(post => new PostViewModel
                        {
                                Id = post.Id.ToString(),
                                Text = post.Text,
                                ImgUrl = post.ImgUrl,
                                CreatedAtUtc = post.CretaedAtUtc,
                                UpdatedAtUtc = post.UpdatedAtUtc,
                                Account = GetAccountBySearchToken(post.AuthorSearchToken),
                                Comments = post.Comments.Select(comment => new CommantViewModel
                                {
                                        Id = comment.Id.ToString(),
                                        Text = comment.Text,
                                        CreatedAtUtc = comment.CreatedAtUtc,
                                        Account = GetAccountBySearchToken(comment.AuthorSearchToken),
                                }).ToList(),
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

                public Account GetAccountBySearchToken(ObjectId token)
                {
                        var user = GetUserBySearchToken(token);

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

                internal User GetUserBySearchToken(ObjectId searchToken)
                {
                        var filter = Builders<User>.Filter.Eq(u => u.Id, searchToken);

                        return _userCollection.Find(filter).FirstOrDefault();
                }

                public void SavePost(string text, string imgUrl, User user)
                {
                        Post post = new Post();

                        post.Text = text;
                        post.ImgUrl = imgUrl;
                        post.CretaedAtUtc = DateTime.UtcNow;
                        post.UpdatedAtUtc = DateTime.UtcNow;
                        post.AuthorSearchToken = user.Id;

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

                public void AddUserTableToComments(User user, string postId,string text)
                {
                        Comment comment = new Comment()
                        {
                                Id = ObjectId.GenerateNewId(),  
                                Text = text,
                                AuthorSearchToken = user.Id,
                                CreatedAtUtc = DateTime.UtcNow,
                        };

                        var filter = Builders<Post>.Filter.Eq(p => p.Id, ObjectId.Parse(postId));
                        var update = Builders<Post>.Update.AddToSet(p => p.Comments, comment);

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
