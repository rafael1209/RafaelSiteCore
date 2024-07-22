using Microsoft.AspNetCore.Mvc;

namespace RafaelSiteCore.Controllers.Hamster
{
        [Route("api/kombat/")]
        [ApiController]
        public class HamsterController : Controller
        {
                [HttpPost]
                public IActionResult getCards(string authToken)
                {

                        return Ok();
                }
        }
}
