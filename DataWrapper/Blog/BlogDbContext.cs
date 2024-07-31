using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using MongoDB.Driver;
using RafaelSiteCore.Model.Blog;
using RafaelSiteCore.Model.Users;
using System.Linq;

namespace RafaelSiteCore.DataWrapper.Blog
{
        public class BlogDbContext
        {
                private readonly MongoClient _mongoClient;

                private const int _pageSizeConst = 10;

                private readonly IMongoDatabase _mongoDatabase;

                private IMongoCollection<Post> _blogCollection;

                private IMongoCollection<Model.Users.User> _userCollection;

                private const string ConstPostsCollection = "Posts";

                private const string ConstUsersCollection = "Users";

                public BlogDbContext(string connectionString, string databaseName)
                {
                        this._mongoClient = new MongoClient(connectionString);

                        this._mongoDatabase = _mongoClient.GetDatabase(databaseName);

                        this._blogCollection = _mongoDatabase.GetCollection<Post>(ConstPostsCollection);

                        this._userCollection = _mongoDatabase.GetCollection<Model.Users.User>(ConstUsersCollection);
                }

                public List<Post> GetUserPosts(ObjectId id)
                {
                        var posts = _blogCollection
                                .Find(post => post.AuthorSearchToken == id)
                                .SortByDescending(post => post.CretaedAtUtc)
                                .ToList();

                        return posts;
                }

                public List<PostView> GetUserPostView(List<Post> posts, Account account)
                {
                        var userPostView = posts.AsParallel()
                                .Select(post => new PostView
                                {
                                        Id = post.Id.ToString(),
                                        Text = post.Text,
                                        ImgUrl = post.ImgUrl,
                                        CreatedAtUtc = post.CretaedAtUtc,
                                        UpdatedAtUtc = post.UpdatedAtUtc,
                                        Account = account,
                                        Comments = post.Comments.AsParallel()
                                        .Select(comment => new CommantView
                                        {
                                                Id = comment.Id.ToString(),
                                                Text = comment.Text,
                                                CreatedAtUtc = comment.CreatedAtUtc,
                                                Account = GetAccountBySearchToken(comment.AuthorSearchToken),
                                        }).ToList(),
                                        Likes = post.Likes.Count(),
                                }).ToList();

                        return userPostView;
                }

                public ProfileView GetUserProfileView(List<PostView> userPosts, Account account, User user, bool IsFollowed)
                {
                        var userProfile = new ProfileView()
                        {
                                Account = account,
                                IsVerified = user.IsVerified,
                                IsBanned = user.IsBanned,
                                Followers = user.Followers.Count(),
                                Following = user.Following.Count(),
                                IsFollowed = IsFollowed,
                                CreatedAtUtc = user.CreatedDateUtc,
                                Posts = userPosts
                        };

                        return userProfile;
                }

                public ProfileView GetUserProfile(string name, string authToken)
                {
                        var requestOwner = GetUserByAuthToken(authToken);

                        var user = GetUserByUsername(name);

                        var userAccount = GetAccountBySearchToken(user.Id);

                        var userPosts = GetUserPosts(user.Id);

                        var userPostView = GetUserPostView(userPosts, userAccount);

                        bool isFollowed = requestOwner.Following.Contains(user.Id);

                        var userProfileModel = GetUserProfileView(userPostView, userAccount, user, isFollowed);

                        return userProfileModel;
                }

                public List<PostView> GetPosts(User user, int page)
                {
                        var posts = _blogCollection.Find(post => true)
                                               .SortByDescending(post => post.CretaedAtUtc)
                                               .Skip((page - 1) * _pageSizeConst)
                                               .Limit(_pageSizeConst)
                                               .ToList();

                        var postViewModels = posts.AsParallel()
                                .Select(post => new PostView
                        {
                                Id = post.Id.ToString(),
                                Text = post.Text,
                                ImgUrl = post.ImgUrl,
                                CreatedAtUtc = post.CretaedAtUtc,
                                UpdatedAtUtc = post.UpdatedAtUtc,
                                Account = GetAccountBySearchToken(post.AuthorSearchToken),
                                Comments = GetPostComments(post),
                                Likes = post.Likes.Count(),
                                IsLiked = post.Likes.Contains(user.Id),
                        }).ToList();

                        return postViewModels;
                }

                public List<CommantView> GetPostComments(Post post)
                {
                        return post.Comments.AsParallel()
                                .Select(comment => new CommantView
                        {
                                Id = comment.Id.ToString(),
                                Text = comment.Text,
                                CreatedAtUtc = comment.CreatedAtUtc,
                                Account = GetAccountBySearchToken(comment.AuthorSearchToken),
                        }).ToList();
                }

                public PostView GetPost(User user, ObjectId postId)
                {
                        var post = _blogCollection.Find(Builders<Post>.Filter.Eq(u => u.Id, postId)).FirstOrDefault();

                        return new PostView
                        {
                                Id = post.Id.ToString(),
                                Text = post.Text,
                                Account = GetAccountBySearchToken(post.AuthorSearchToken),
                                CreatedAtUtc = post.CretaedAtUtc,
                                UpdatedAtUtc = post.UpdatedAtUtc,
                                ImgUrl = post.ImgUrl,
                                Likes = post.Likes.Count(),
                                IsLiked = post.Likes.Contains(user.Id),
                                Comments = GetPostComments(post)
                        };
                }

                public Account GetAccountByDiscordID(ulong discordID)
                {
                        var user = GetUserByIdDiscord(discordID);

                        return new Account()
                        {
                                SearchToken = user.Id.ToString(),
                                AvatarUrl = user.AvatarUrl,
                                Username = user.Name,
                                IsVerified = user.IsVerified,
                        };
                }

                public Account GetAccountBySearchToken(ObjectId token)
                {
                        var user = GetUserBySearchToken(token);

                        if(user == null)
                                return null!;

                        return new Account()
                        {
                                SearchToken = user!.Id.ToString(),
                                AvatarUrl = user.AvatarUrl,
                                Username = user.Name,
                                IsVerified = user.IsVerified,
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

                public User GetUserByAuthToken(string authToken)
                {
                        var filter = Builders<User>.Filter.Eq(u => u.AuthToken, authToken);

                        return _userCollection.Find(filter).FirstOrDefault();
                }

                public Model.Users.User GetUserByUsername(string name)
                {
                        var filter = Builders<User>.Filter.Eq(u => u.Name, name);

                        return _userCollection.Find(filter).FirstOrDefault();
                }

                public void SavePost(string text, string fileUrl, User user)
                {
                        Post post = new Post();

                        post.Text = text;
                        post.ImgUrl = fileUrl;
                        post.CretaedAtUtc = DateTime.UtcNow;
                        post.UpdatedAtUtc = DateTime.UtcNow;
                        post.AuthorSearchToken = user.Id;

                        _blogCollection.InsertOne(post);
                }

                public void LikePost(User user, ObjectId postId)
                {
                        var filter = Builders<Post>.Filter.Eq(p => p.Id, postId);
                        var update = Builders<Post>.Update.AddToSet(p => p.Likes, user.Id);

                        _blogCollection.UpdateOne(filter, update);
                }

                public void LikeComment(User user, ObjectId postId, ObjectId commentId)
                {
                        var filter = Builders<Post>.Filter.Eq(p => p.Id, postId) &
                                     Builders<Post>.Filter.ElemMatch(p => p.Comments, c => c.Id == commentId);

                        var update = Builders<Post>.Update.AddToSet(p => p.Comments[1].Likes, user.Id);

                        _blogCollection.UpdateOne(filter, update);
                }


                public void FollowUser(User me, string name)
                {
                        var user = GetUserByUsername(name);

                        AddObjectIdToUserFollow(me, user);
                }

                public void AddObjectIdToUserFollow(User me, User user)
                {
                        var filter = Builders<User>.Filter.Eq(p => p.Id, user.Id);
                        var update = Builders<User>.Update
                                .AddToSet(p => p.Followers, me.Id);

                        var userFilter = Builders<User>.Filter.Eq(p => p.Id, me.Id);
                        var userUpdate = Builders<User>.Update
                                .AddToSet(p => p.Following, user.Id);

                        _userCollection.UpdateOne(filter, update);
                        _userCollection.UpdateOne(userFilter, userUpdate);
                }

                public CommantView AddUserTableToCommentsAndReturn(User user, string postId, string text)
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

                        return new CommantView
                        {
                                Id = comment.Id.ToString(),
                                Text = text,
                                Account = GetAccountBySearchToken(user.Id),
                                CreatedAtUtc = comment.CreatedAtUtc,
                                Likes = comment.Likes.Count(),
                                IsLiked = comment.Likes.Contains(user.Id)
                        };
                }

                public void UnlikePost(User user, ObjectId postId)
                {
                        var filter = Builders<Post>.Filter.Eq(p => p.Id, postId);
                        var update = Builders<Post>.Update.Pull(p => p.Likes, user.Id);

                        _blogCollection.UpdateOne(filter, update);
                }
        }
}
