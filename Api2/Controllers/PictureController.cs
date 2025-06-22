using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api2.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PictureController : ControllerBase
    {
        [Authorize(Policy ="ReadData")]
        [HttpGet]
        public IActionResult Pictures()
        {
            var pictures = new List<Models.Picture>
            {
                new Models.Picture { Id = 1, Name = "Aslan", Link = "https://example.com/resim1.jpg" },
                new Models.Picture { Id = 2, Name = "Fare", Link = "https://example.com/resim2.jpg" },
                new Models.Picture { Id = 3, Name = "Kurt", Link = "https://example.com/resim3.jpg" }
            };
            return Ok(pictures);
        }
    }
}
