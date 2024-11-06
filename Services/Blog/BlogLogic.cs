using MongoDB.Bson;
using RafaelSiteCore.DataWrapper.Authorize;
using RafaelSiteCore.DataWrapper.Blog;
using RafaelSiteCore.Interfaces;
using RafaelSiteCore.Model.Blog;
using RafaelSiteCore.Model.Users;
using RafaelSiteCore.Services.Auth;
using RafaelSiteCore.Services.GoogleDrive;

namespace RafaelSiteCore.Services.Blog
{
        public class BlogLogic
        {
                private BlogDbContext _dbContext;

                private readonly IStorage _storageService;

                private AuthorizeDbContext _authorizeDbContext;

                public BlogLogic(BlogDbContext dbContext, AuthorizeDbContext authorizeDbContext, IStorage storageService)
                {
                        _storageService = storageService;

                        _dbContext = dbContext;

                        _authorizeDbContext = authorizeDbContext;
                }

                public ProfileView GetUserProfile(string name, User user)
                {
                        return _dbContext.GetUserProfile(name, user);
                }

                public List<PostDto> GetPosts(User user ,int page)
                {
                        return _dbContext.GetPosts(user, page);
                }

                public CommantView AddCommentAndReturn(User user, string postId, string comment)
                {
                        return _dbContext.AddUserTableToCommentsAndReturn(user, postId, comment);
                }

                public void AddFollow(User user,string username)
                {
                        _dbContext.FollowUser(user, username);
                }

                public void RemoveFollow(User user, string username)
                {
                        _dbContext.UnFollowUser(user, username);
                }

                public async Task AddPostAsync(string title, IFormFile file, User user)
                {
                        string fileUrl = "";

                        if (file!=null)
                                fileUrl = await _storageService.UploadFile(file);

                        _dbContext.SavePost(title, fileUrl, user);
                }

                public void LikePost(User user, ObjectId postId)
                {
                        _dbContext.LikePost(user, postId);
                }

                public void LikePostComment(User user, ObjectId postId,ObjectId commentId)
                {
                        _dbContext.LikeComment(user, postId, commentId);
                }

                public void UnLikePostComment(User user, ObjectId postId, ObjectId commentId)
                {
                        _dbContext.UnLikeComment(user, postId, commentId);
                }

                public void UnlikePost(User user, ObjectId postId)
                {
                        _dbContext.UnlikePost(user, postId);
                }

                public Account GetAccount(ulong discordId)
                {
                        return _authorizeDbContext.GetAccountByIdDiscord(discordId);
                }

                public PostView GetPost(User user,ObjectId postId)
                {
                        return _dbContext.GetPost(user,postId);
                }
        }
}
