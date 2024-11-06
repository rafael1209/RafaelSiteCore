using MongoDB.Bson;
using RafaelSiteCore.Model.Authorize;
using System.Text.Json.Serialization;

namespace RafaelSiteCore.Model.Blog
{
        public class CreateBlogRequest
        {
                public string Text { get; set; } = string.Empty;

                public IFormFile? File { get; set; }
        }
}
