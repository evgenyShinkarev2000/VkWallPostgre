using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using VkWallPostgre.Dto;
using VkWallPostgre.Infostracture;

namespace VkWallPostgre.Controllers
{
    [Route("api/Auth")]
    public class AuthController : Controller
    {
        private readonly VkOpenIdHelper openIdHelper;
        private readonly IDistributedCache cache;

        public AuthController(VkOpenIdHelper openIdHelper, IDistributedCache cache)
        {
            this.openIdHelper = openIdHelper;
            this.cache = cache;
        }

        [HttpGet("AcceptCode")]
        public async Task<IActionResult> AcceptCode([FromQuery] string code)
        {
            if (code == null)
            {
                return BadRequest();
            }
            var redirectUri = BuildRedirectUri();
            var response = await new HttpClient().GetAsync(openIdHelper.BuildTokenByCodeString(redirectUri, code));
            var token = await response.Content.ReadFromJsonAsync<VkToken>();
            if (token == null)
            {
                return BadRequest();
            }
            cache.SetVkToken(token);

            return Ok("You has been authorized, repeat request " + token.Token);
        }

        [HttpGet("CodeFlow")]
        public IActionResult CodeFlow()
        {
            var authLink = openIdHelper.BuildCodeFlowString(BuildRedirectUri());
            return Unauthorized($"use link for auth {authLink}");
        }

        private string BuildRedirectUri() => Request.Scheme + "://" + Request.Host.Value + "/api/Auth/AcceptCode";
    }
}
