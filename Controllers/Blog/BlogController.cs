using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using RafaelSiteCore.Middlewere;
using RafaelSiteCore.Model.Blog;
using RafaelSiteCore.Services.Auth;
using RafaelSiteCore.Services.Blog;
using RafaelSiteCore.Services.Logger;

namespace RafaelSiteCore.Controllers.Blog;

[Route("api/v1/blog")]
[ApiController]
public class BlogController(BlogLogic blogLogic, DiscordAuthLogic discordAuthLogic, DiscordAlert discordWebhook)
    : Controller
{
    [HttpGet]
    public IActionResult GetAllPostsAsync([FromQuery] int page = 1)
    {
        Request.Headers.TryGetValue("Authorization", out var token);

        var user = discordAuthLogic.GetUser(token!);

        return Json(blogLogic.GetPosts(user, page));
    }

    [HttpPost("{postId}/comment")]
    [AuthMiddleware]
    public IActionResult AddComment([FromRoute] string postId, CommentRequest request)
    {
        if (string.IsNullOrEmpty(request.comment))
            return BadRequest("Comment is empty.");

        Request.Headers.TryGetValue("Authorization", out var token);

        var user = discordAuthLogic.GetUser(token!);

        if (user.IsBanned)
        {
            discordWebhook.WarningLogger("Banned user", $"User: {user.Name}", user.AvatarUrl);

            return BadRequest("User is banned");
        }

        return Ok(blogLogic.AddCommentAndReturn(user, postId, request.comment));
    }

    [HttpGet("post/{postId}")]
    public IActionResult GetPost([FromRoute] string postId)
    {
        if (!ObjectId.TryParse(postId, out ObjectId postObjectId))
            return BadRequest("Invalid PostId format.");

        Request.Headers.TryGetValue("Authorization", out var token);

        var user = discordAuthLogic.GetUser(token!);

        return Ok(blogLogic.GetPost(user, postObjectId));
    }

    [HttpPut("{username}/follow")]
    [AuthMiddleware]
    public IActionResult Follow([FromRoute] string username)
    {
        Request.Headers.TryGetValue("Authorization", out var token);

        var user = discordAuthLogic.GetUser(token!);

        blogLogic.AddFollow(user, username);

        return Ok();
    }

    [HttpDelete("{username}/follow")]
    [AuthMiddleware]
    public IActionResult UnFollow([FromRoute] string username)
    {
        Request.Headers.TryGetValue("Authorization", out var token);

        var user = discordAuthLogic.GetUser(token!);

        blogLogic.RemoveFollow(user, username);

        return Ok();
    }

    [HttpPost]
    [AuthMiddleware]
    public IActionResult CreatePost(CreateBlogRequest createBlogRequest)
    {
        Request.Headers.TryGetValue("Authorization", out var token);

        var user = discordAuthLogic.GetUser(token!);

        if (user.IsBanned)
        {
            discordWebhook.WarningLogger("Banned user", $"User: {user.Name}", user.AvatarUrl);

            return BadRequest("User is banned");
        }

        blogLogic.AddPostAsync(createBlogRequest.Text, createBlogRequest.File, user);

        discordWebhook.SendNewPostAlert();

        return Ok();
    }

    [HttpPut("{postId}/like")]
    [AuthMiddleware]
    public IActionResult LikePost([FromRoute] string postId)
    {
        Request.Headers.TryGetValue("Authorization", out var token);

        if (!ObjectId.TryParse(postId, out ObjectId postObjectId))
            return BadRequest("Invalid PostId format.");

        var user = discordAuthLogic.GetUser(token!);

        blogLogic.LikePost(user, ObjectId.Parse(postId));

        return Ok();
    }

    [HttpPut("{postId}/{commentId}/like")]
    [AuthMiddleware]
    public IActionResult LikePostComment([FromRoute] string postId, [FromRoute] string commentId)
    {
        Request.Headers.TryGetValue("Authorization", out var token);

        if (!ObjectId.TryParse(postId, out ObjectId postObjectId)
            || !ObjectId.TryParse(commentId, out ObjectId commentObjectId))
            return BadRequest("Invalid PostId or CommentId format.");

        var user = discordAuthLogic.GetUser(token!);

        blogLogic.LikePostComment(user, postObjectId, commentObjectId);

        return Ok();
    }

    [HttpDelete("{postId}/{commentId}/like")]
    [AuthMiddleware]
    public IActionResult UnLikePostComment([FromRoute] string postId, [FromRoute] string commentId)
    {
        Request.Headers.TryGetValue("Authorization", out var token);

        if (!ObjectId.TryParse(postId, out ObjectId postObjectId)
            || !ObjectId.TryParse(commentId, out ObjectId commentObjectId))
            return BadRequest("Invalid PostId or CommentId format.");

        var user = discordAuthLogic.GetUser(token!);

        blogLogic.UnLikePostComment(user, postObjectId, commentObjectId);

        return Ok();
    }

    [HttpDelete("{postId}/like")]
    [AuthMiddleware]
    public IActionResult UnlikePost([FromRoute] string postId)
    {
        Request.Headers.TryGetValue("Authorization", out var token);

        if (!ObjectId.TryParse(postId, out ObjectId postObjectId))
            return BadRequest("Invalid PostId format.");

        var user = discordAuthLogic.GetUser(token!);

        blogLogic.UnlikePost(user, ObjectId.Parse(postId));

        return Ok();
    }

    [HttpGet("profile/{name}")]
    //[AuthMiddleware]
    public IActionResult GetUserProfile([FromRoute] string name)
    {
        if (Request.Headers.TryGetValue("Authorization", out var token))
        {
            var user = discordAuthLogic.GetUser(token);

            if (user.IsBanned)
            {
                discordWebhook.WarningLogger("Banned user", $"User: {user.Name}", user.AvatarUrl);

                return BadRequest("User is banned");
            }

            return Json(blogLogic.GetUserProfile(name, user));
        }

        return Json(blogLogic.GetUserProfile(name));
    }
}