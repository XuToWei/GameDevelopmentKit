
using Microsoft.AspNetCore.Mvc;

namespace ET
{
    [ApiController]
    [Route("api/[controller]")]
    public class VersionController : ControllerBase
    {
        private readonly VersionService _versionService;

        public VersionController(VersionService versionService)
        {
            _versionService = versionService;
        }

        /// <summary>
        /// Get version info for client (public API)
        /// </summary>
        [HttpGet("{platform}")]
        public IActionResult GetVersion(string platform, [FromQuery] string env = null)
        {
            var versionInfo = _versionService.GetClientVersionInfo(platform, env);

            if (versionInfo == null)
            {
                return NotFound(new { error = "No active version found for this platform" });
            }

            return Ok(versionInfo);
        }
    }
}
