using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MiniApp1.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class StockController : ControllerBase
    { 
        [HttpGet]
        public IActionResult GetStock()
        {
            var username = HttpContext.User.Identity.Name;
            var userIdClaim = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            return Ok($"UserName:{username}-UserId:{userIdClaim.Value}");
        }
    }
}