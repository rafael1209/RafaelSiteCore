﻿using MongoDB.Bson;
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

                public ProfileView GetUserProfile(string name, string authToken)
                {
                        return _dbContext.GetUserProfile(name, authToken);
                }

                public List<PostView> GetPosts(User user ,int page)
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

                public async Task AddPostAsync(string title, IFormFile file, User user)
                {
                        string fileUrl = await _storageService.UploadFile(file);
                        _dbContext.SavePost(title, fileUrl, user);
                }


                public void LikePost(User user, ObjectId postId)
                {
                        _dbContext.LikePost(user, postId);
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
