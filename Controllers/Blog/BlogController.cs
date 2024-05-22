using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using RafaelSiteCore.DataWrapper.Blog;
using RafaelSiteCore.Model.Authorize;
using RafaelSiteCore.Model.Blog;
using RafaelSiteCore.Services.Auth;
using RafaelSiteCore.Services.Blog;
using System.ComponentModel.DataAnnotations;

namespace RafaelSiteCore.Controllers.Blog
{
        [Route("api/[controller]")]
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

                [HttpGet("GetAllPosts")]
                public IActionResult GetAllPosts()
                {


                        return Json(_blogLogic.GetAllPosts());
                }

                [HttpPost("CreatePost")]
                public IActionResult CreatePost([FromBody] CreateBlogRequest BlogRequest)
                {
                        var user = _authLogic.IsUserExist(ObjectId.Parse(BlogRequest.Credential.AuthToken));

                        if (user == null)
                                return Unauthorized();

                        _blogLogic.AddPost(BlogRequest.Title, BlogRequest.body, BlogRequest.ImgUrl, user);

                        return Ok();
                }

                [HttpPost("LikePost")]
                public IActionResult LikePost([FromBody] UserCredential credential, string postId)
                {
                        var user = _authLogic.IsUserExist(ObjectId.Parse(credential.AuthToken));

                        if (user == null)
                                return Unauthorized();

                        _blogLogic.LikePost(user, ObjectId.Parse(postId));

                        return Ok();
                }
        }
}
