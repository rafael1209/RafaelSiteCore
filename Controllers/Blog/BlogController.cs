using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using RafaelSiteCore.DataWrapper.Blog;
using RafaelSiteCore.Middlewere;
using RafaelSiteCore.Model.Authorize;
using RafaelSiteCore.Model.Blog;
using RafaelSiteCore.Services.Auth;
using RafaelSiteCore.Services.Blog;
using System.ComponentModel.DataAnnotations;

namespace RafaelSiteCore.Controllers.Blog
{
        [Route("api/v1/blog/")]
        [ApiController]
        public class BlogController : Controller
        {
                private BlogLogic _blogLogic;
                private DiscordAuthLogic _authLogic;

                public BlogController(BlogLogic blogLogic, DiscordAuthLogic discordAuthLogic)
                {
                        _blogLogic = blogLogic;

                        _authLogic = discordAuthLogic;
                }

                [HttpGet]
                [AuthMiddleware]
                public IActionResult GetAllPosts([FromQuery] int page)
                {
                        Request.Headers.TryGetValue("Authorization", out var token);

                        var user = _authLogic.GetUser(token!);

                        if(page ==0)
                        page = 1;

                        return Json(_blogLogic.GetPosts(user, page));
                }

                [HttpPost("{postId}/comment")]
                [AuthMiddleware]
                public IActionResult AddComment([FromRoute] string postId,CommentRequest request)
                {
                        if (string.IsNullOrEmpty(request.comment))
                                return BadRequest("Comment is empty.");

                        Request.Headers.TryGetValue("Authorization", out var token);

                        var user = _authLogic.GetUser(token!);

                        return Ok(_blogLogic.AddCommentAndReturn(user, postId, request.comment));
                }

                [HttpGet("post/{postId}")]
                [AuthMiddleware]
                public IActionResult GetPost([FromRoute] string postId)
                {
                        if (!ObjectId.TryParse(postId, out ObjectId postObjectId))
                                return BadRequest("Invalid PostId format.");

                        Request.Headers.TryGetValue("Authorization", out var token);

                        var user = _authLogic.GetUser(token!);

                        return Ok(_blogLogic.GetPost(user, postObjectId));
                }

                [HttpPost("{username}/follow")]
                [AuthMiddleware]
                public IActionResult AddComment([FromRoute] string username)
                {
                        Request.Headers.TryGetValue("Authorization", out var token);

                        var user = _authLogic.GetUser(token!);

                        _blogLogic.AddFollow(user, username);

                        return Ok();
                }

                [HttpPost]
                [AuthMiddleware]
                public IActionResult CreatePost(CreateBlogRequest createBlogRequest)
                {
                        Request.Headers.TryGetValue("Authorization", out var token);

                        var user = _authLogic.GetUser(token!);

                        _blogLogic.AddPostAsync(createBlogRequest.Text, createBlogRequest.File, user);

                        return Ok();
                }

                [HttpPut("{postId}/like")]
                [AuthMiddleware]
                public IActionResult LikePost([FromRoute] string postId)
                {
                        Request.Headers.TryGetValue("Authorization", out var token);

                        if (!ObjectId.TryParse(postId, out ObjectId postObjectId))
                                return BadRequest("Invalid PostId format.");

                        var user = _authLogic.GetUser(token!);

                        _blogLogic.LikePost(user, ObjectId.Parse(postId));

                        return Ok();
                }

                [HttpPut("{postId}/{commentId}/like")]
                public IActionResult LikePostComment([FromRoute] string postId, [FromRoute] string commentId)
                {
                        Request.Headers.TryGetValue("Authorization", out var token);

                        if (!ObjectId.TryParse(postId, out ObjectId postObjectId) 
                                || !ObjectId.TryParse(commentId, out ObjectId commentObjectId))
                                return BadRequest("Invalid PostId or CommentId format.");

                        var user = _authLogic.GetUser(token!);

                        _blogLogic.LikePostComment(user, postObjectId,commentObjectId);

                        return Ok();
                }

                [HttpDelete("{postId}/like")]
                [AuthMiddleware]
                public IActionResult UnlikePost([FromRoute] string postId)
                {
                        Request.Headers.TryGetValue("Authorization", out var token);

                        if (!ObjectId.TryParse(postId, out ObjectId postObjectId))
                                return BadRequest("Invalid PostId format.");

                        var user = _authLogic.GetUser(token!);

                        _blogLogic.UnlikePost(user, ObjectId.Parse(postId));

                        return Ok();
                }

                [HttpGet("profile/{name}")]
                [AuthMiddleware]
                public IActionResult GetUserProfile([FromRoute] string name)
                {
                        Request.Headers.TryGetValue("Authorization", out var authToken);

                        return Json(_blogLogic.GetUserProfile(name, authToken!));
                }
        }
}