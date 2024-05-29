using MongoDB.Bson;
using RafaelSiteCore.DataWrapper.Authorize;
using RafaelSiteCore.DataWrapper.Blog;
using RafaelSiteCore.Model.Blog;
using RafaelSiteCore.Model.Users;

namespace RafaelSiteCore.Services.Blog
{
        public class BlogLogic
        {
                private BlogDbContext _dbContext;

                private AuthorizeDbContext _authorizeDbContext;

                public BlogLogic(BlogDbContext dbContext,AuthorizeDbContext authorizeDbContext)
                {
                        _dbContext = dbContext;

                        _authorizeDbContext = authorizeDbContext;
                }

                public List<PostSummary> GetAllPosts()
                {
                        return _dbContext.GetPosts();
                }

                public void AddPost(string title, string body, string ImgUrl, User user)
                {
                        _dbContext.SavePost(title, body, ImgUrl, user);
                }

                public void LikePost(User user,ObjectId postId)
                {
                        _dbContext.LikePost(user, postId);
                }

                public Account GetAccount(ulong discordId)
                {
                        return _authorizeDbContext.GetAccountByIdDiscord(discordId);
                }
        }
}
