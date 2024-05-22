using MongoDB.Bson;
using RafaelSiteCore.Model.Authorize;
using System.Text.Json.Serialization;

namespace RafaelSiteCore.Model.Blog
{
        public class CreateBlogRequest
        {
                public UserCredential Credential { get; set; } = new UserCredential();

                public string Title { get; set; } = string.Empty;

                public string body { get; set; } = string.Empty;

                public string ImgUrl { get; set; } = string.Empty;
        }
}
