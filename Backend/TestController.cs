using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Route("api")]
    public class TestController : ControllerBase
    {
        [HttpGet("get-status")]
        public IActionResult GetStatus()
        {
            return Ok("service đã sẵn sàng");
        }

        [HttpGet("test-claims")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public IActionResult TestClaims()
        {
            var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            return Ok(new {
                claims,
                isInAdminRole = User.IsInRole("Admin"),
                roleClaimType = User.Identity is System.Security.Claims.ClaimsIdentity id ? id.RoleClaimType : "none"
            });
        }
    }
}