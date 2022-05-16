using AGA.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AGA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SecurityController : ControllerBase
    {

        /// <summary>
        /// Get User by Email and return new token
        /// </summary>
        /// <returns></returns>
        [HttpPost("GetToken")]
        public IActionResult GetToken()
        {
            var remoteIpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            var userAgent = Request.Headers["User-Agent"].ToString();
            var token = TokenGenerator.GenerateToken(remoteIpAddress, userAgent);
           return Ok(token);

        }
    }
}
