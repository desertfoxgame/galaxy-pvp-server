using GalaxyPvP.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace GalaxyPvP.Api.Controllers
{
    public class BaseController: ControllerBase
    {
        public IActionResult ReturnFormatedResponse<T>(ApiResponse<T> response)
        {
            if (response.Success)
            {
                return Ok(response.Data);
            }
            return StatusCode(response.StatusCode, response.Errors);
        }
    }
}
