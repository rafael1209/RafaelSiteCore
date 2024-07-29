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

                        return Json(_blogLogic.GetPosts(user, page));
                }

                [HttpPost("{postId}/comment")]
                [AuthMiddleware]
                public IActionResult AddComment([FromRoute] string postId,CommentRequest request)
                {
                        Request.Headers.TryGetValue("Authorization", out var token);

                        var user = _authLogic.GetUser(token!);

                        _blogLogic.AddComment(user, postId, request.comment);

                        return Ok();
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
                public IActionResult LikePost([FromRoute] string postId)
                {
                        Request.Headers.TryGetValue("Authorization", out var token);

                        if (!ObjectId.TryParse(postId, out ObjectId postObjectId))
                                return BadRequest("Invalid PostId format.");

                        var user = _authLogic.GetUser(token!);

                        if (user == null)
                                return Unauthorized();

                        _blogLogic.LikePost(user, ObjectId.Parse(postId));

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