using MongoDB.Bson;
using RafaelSiteCore.DataWrapper.Authorize;
using RafaelSiteCore.DataWrapper.Blog;
using RafaelSiteCore.Model.Blog;
using RafaelSiteCore.Model.Users;
using RafaelSiteCore.Services.Auth;
using RafaelSiteCore.Services.GoogleDrive;

namespace RafaelSiteCore.Services.Blog
{
        public class BlogLogic
        {
                private BlogDbContext _dbContext;

                private readonly GoogleDriveService _googleDriveService;

                private AuthorizeDbContext _authorizeDbContext;

                public BlogLogic(BlogDbContext dbContext, AuthorizeDbContext authorizeDbContext, GoogleDriveService googleDriveService)
                {
                        _googleDriveService = googleDriveService;
                        
                        _dbContext = dbContext;

                        _authorizeDbContext = authorizeDbContext;
                }

                public List<PostViewModel> GetAllPosts()
                {
                        return _dbContext.GetPosts();
                }

                public void AddComment(User user,string postId, string comment)
                {
                        _dbContext.AddUserTableToComments(user, postId, comment);
                }


                public async Task AddPostAsync(string title, IFormFile file, User user)
                {
                        string fileUrl = await _googleDriveService.UploadFile(file);
                        _dbContext.SavePost(title, fileUrl, user);
                }


                public void LikePost(User user,ObjectId postId)
                {
                        _dbContext.LikePost(user, postId);
                }

                public void UnlikePost(User user,ObjectId postId)
                {
                        _dbContext.UnlikePost(user, postId);
                }
                public Account GetAccount(ulong discordId)
                {
                        return _authorizeDbContext.GetAccountByIdDiscord(discordId);
                }
        }
}
