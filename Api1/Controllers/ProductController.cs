using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api1.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        [Authorize(Policy ="ReadData")]
        [HttpGet]
        public IActionResult Products()
        {
            var products = new List<Models.Product>
            {
                new Models.Product { Id = 1, Name = "Kalem", Price = 10.99m, InStock = true },
                new Models.Product { Id = 2, Name = "Silgi", Price = 20.99m, InStock = false },
                new Models.Product { Id = 3, Name = "Defter", Price = 30.99m, InStock = true }
            };
            return Ok(products);
        }
    }
}
