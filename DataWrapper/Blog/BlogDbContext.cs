﻿using MongoDB.Bson;
using MongoDB.Driver;
using RafaelSiteCore.Model.Blog;
using RafaelSiteCore.Model.Users;
using Microsoft.Extensions.Caching.Memory;
using System.Globalization;
using System.Linq;

namespace RafaelSiteCore.DataWrapper.Blog
{
    public class BlogDbContext
    {
        private readonly MongoClient _mongoClient;
        private readonly IMemoryCache _cache;
        private const int _pageSizeConst = 10;
        private readonly IMongoDatabase _mongoDatabase;
        private IMongoCollection<Post> _blogCollection;
        private IMongoCollection<User> _userCollection;
        private const string ConstPostsCollection = "Posts";
        private const string ConstUsersCollection = "Users";

        public BlogDbContext(string connectionString, string databaseName, IMemoryCache cache)
        {
            this._mongoClient = new MongoClient(connectionString);
            this._mongoDatabase = _mongoClient.GetDatabase(databaseName);
            this._blogCollection = _mongoDatabase.GetCollection<Post>(ConstPostsCollection);
            this._userCollection = _mongoDatabase.GetCollection<Model.Users.User>(ConstUsersCollection);
            this._cache = cache;
        }

        public List<Post> GetUserPosts(ObjectId id)
        {
            var posts = _blogCollection
                .Find(post => post.AuthorSearchToken == id)
                .SortByDescending(post => post.CretaedAtUtc)
                .Limit(_pageSizeConst)
                .ToList();

            return posts;
        }


        public List<PostDto> GetUserPostsView(List<Post> posts, Account account, ObjectId? ownerId = null)
        {
            var userPostView = posts.AsParallel()
                    .Select(post => new PostDto
                    {
                        Id = post.Id.ToString(),
                        Text = post.Text,
                        ImgUrl = post.ImgUrl,
                        CreatedAtUtc = post.CretaedAtUtc,
                        UpdatedAtUtc = post.UpdatedAtUtc,
                        Account = account,
                        Comments = post.Comments.Count(),
                        Likes = post.Likes.Count(), 
                        IsLiked = post.Likes.Contains(ownerId??ObjectId.GenerateNewId())//TODO: Fix this shit!
                    }).ToList();

            return userPostView;
        }

        public ProfileView GetUserProfileView(List<PostDto> userPosts, Account account, User user, bool IsFollowed = false, bool IsOwner = false)
        {
            var userProfile = new ProfileView()
            {
                Account = account,
                IsAuthor = IsOwner,
                IsBanned = user.IsBanned,
                Followers = user.Followers.Count(),
                Following = user.Following.Count(),
                IsFollowed = IsFollowed,
                CreatedAtUtc = user.CreatedDateUtc,
                Posts = userPosts
            };

            return userProfile;
        }

        public ProfileView GetUserProfile(string name, User? userOwner = null)
        {
            ProfileView userProfileModel = new ProfileView();
            List<PostDto> postsList = new List<PostDto>();

            var user = GetUserByUsername(name);

            var userAccount = GetAccountBySearchToken(user.Id);

            var userPosts = GetUserPosts(user.Id);

            if (userOwner != null)
            {
                var requestOwner = GetUserBySearchToken(userOwner.Id);

                var isFollowed = requestOwner.Following.Contains(user.Id);

                var isOwner = requestOwner.Id == user.Id;

                postsList = GetUserPostsView(userPosts, userAccount);

                return GetUserProfileView(postsList, userAccount, user, isFollowed,isOwner);
            }

            postsList = GetUserPostsView(userPosts, userAccount);

            userProfileModel = GetUserProfileView(postsList, userAccount, user);

            return userProfileModel;
        }

        public List<PostDto> GetPosts(User? user, int page)
        {
            var posts = _blogCollection.Find(post => true)
                                       .SortByDescending(post => post.CretaedAtUtc)
                                       .Skip((page - 1) * _pageSizeConst)
                                       .Limit(_pageSizeConst)
                                       .ToList();

            var postView = posts.AsParallel()
               .Select(post => new PostDto
               {
                   Id = post.Id.ToString(),
                   Text = post.Text,
                   ImgUrl = post.ImgUrl,
                   CreatedAtUtc = post.CretaedAtUtc,
                   UpdatedAtUtc = post.UpdatedAtUtc,
                   Account = GetAccountBySearchToken(post.AuthorSearchToken),
                   Comments = post.Comments.Count,
                   Likes = post.Likes.Count(),
                   IsLiked = user != null && post.Likes.Contains(user.Id),
               }).ToList();

            return postView;
        }

        //public int GetCashedCommentsCount(Post post)
        //{
        //    string cacheKey = $"CommentsCount-{post.Id}";

        //    if (!_cache.TryGetValue(cacheKey, out int commentCount))
        //    {
        //        commentCount = post.Comments.Count;

        //        var cacheEntryOptions = new MemoryCacheEntryOptions()
        //            .SetSlidingExpiration(TimeSpan.FromMinutes(30));

        //        _cache.Set(cacheKey, commentCount, cacheEntryOptions);
        //    }

        //    return commentCount;
        //}

        public List<CommantView> GetPostComments(User user, Post post)
        {
            var commentsList = post.Comments
                .AsParallel()
                .OrderBy(comment => comment.CreatedAtUtc)
                .Reverse()
                .Select(comment => new CommantView
                {
                    Id = comment.Id.ToString(),
                    Text = comment.Text,
                    CreatedAtUtc = comment.CreatedAtUtc,
                    Account = GetAccountBySearchToken(comment.AuthorSearchToken),
                    Likes = comment.Likes.Count(),
                    IsLiked = comment.Likes.Contains(user.Id),
                }).ToList();

            return commentsList;
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
                Comments = GetPostComments(user, post)
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

        public Account GetAccountBySearchToken(ObjectId searchToken)
        {
            string cacheKey = $"Account-{searchToken}";

            if (!_cache.TryGetValue(cacheKey, out Account? account))
            {
                var user = GetUserBySearchToken(searchToken);

                if (user == null)
                    return null!;

                account = new Account()
                {
                    SearchToken = user!.Id.ToString(),
                    AvatarUrl = user.AvatarUrl,
                    Username = user.Name,
                    IsVerified = user.IsVerified,
                };

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(30));

                _cache.Set(cacheKey, account, cacheEntryOptions);
            }

            return account ?? throw new Exception($"Account is null: {searchToken}");
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
            var post = _blogCollection.Find(p => p.Id == postId).FirstOrDefault();

            if (post != null)
            {
                var comment = post.Comments.FirstOrDefault(c => c.Id == commentId);

                if (comment != null)
                {
                    if (!comment.Likes.Contains(user.Id))
                    {
                        comment.Likes.Add(user.Id);

                        _blogCollection.ReplaceOne(p => p.Id == postId, post);
                    }
                }
                else
                {
                    throw new ArgumentException("Comment not found.");
                }
            }
            else
            {
                throw new ArgumentException("Post not found.");
            }
        }

        public void UnLikeComment(User user, ObjectId postId, ObjectId commentId)
        {
            var post = _blogCollection.Find(p => p.Id == postId).FirstOrDefault();

            if (post != null)
            {
                var comment = post.Comments.FirstOrDefault(c => c.Id == commentId);

                if (comment != null)
                {
                    if (comment.Likes.Contains(user.Id))
                    {
                        comment.Likes.Remove(user.Id);

                        _blogCollection.ReplaceOne(p => p.Id == postId, post);
                    }
                }
                else
                {
                    throw new ArgumentException("Comment not found.");
                }
            }
            else
            {
                throw new ArgumentException("Post not found.");
            }
        }


        public void FollowUser(User me, string name)
        {
            var user = GetUserByUsername(name);

            AddObjectIdToUserFollow(me, user);
        }

        public void UnFollowUser(User me, string name)
        {
            var user = GetUserByUsername(name);

            RemoveObjectIdToUserFollow(me, user);
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

        public void RemoveObjectIdToUserFollow(User me, User user)
        {
            var filter = Builders<User>.Filter.Eq(p => p.Id, user.Id);
            var update = Builders<User>.Update.Pull(p => p.Followers, me.Id);

            var userFilter = Builders<User>.Filter.Eq(p => p.Id, me.Id);
            var userUpdate = Builders<User>.Update.Pull(p => p.Following, user.Id);

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
