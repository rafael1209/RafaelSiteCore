using MongoDB.Bson;
using RafaelSiteCore.Model.Authorize;
using System.Text.Json.Serialization;

namespace RafaelSiteCore.Model.Blog
{
        public class CreateBlogRequest
        {
                public required string Text { get; set; }

                public IFormFile? File { get; set; }
        }
}
