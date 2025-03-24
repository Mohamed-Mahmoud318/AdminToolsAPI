using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Admin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {

        private static int _eventsCount = 10; 
        private static int _ordersCount = 25;  
        private static int _usersCount = 50;   
        private static bool _maintenanceMode = false;

        private bool IsUserAdmin()
        {
            var isAdminClaim = User.Claims.FirstOrDefault(c => c.Type == "IsAdmin")?.Value;
            return isAdminClaim == "True"; 
        }
        
        [HttpGet("check")]
        public IActionResult CheckAdminAccess()
        {
            if (!IsUserAdmin())
                return Forbid(); 

            return Ok(new { message = "Welcome, Admin!" });
        }
        [HttpPost("maintenance/toggle")]
        public IActionResult ToggleMaintenanceMode()
        {
            if (!IsUserAdmin())
                return Forbid(); 

            _maintenanceMode = !_maintenanceMode; 

            return Ok(new { maintenanceMode = _maintenanceMode, message = "Maintenance mode updated." });
        }
       
        [HttpGet("maintenance/status")]
        public IActionResult GetMaintenanceStatus()
        {
            return Ok(new { maintenanceMode = _maintenanceMode });
        }

        
        [HttpGet("GetSiteStats")]
        public IActionResult GetSiteStats()
        {
            if (!IsUserAdmin())
                return Forbid(); 

           
            if (_maintenanceMode)
            {
                return StatusCode(503, new
                {
                    status = 503,
                    message = "Service is temporarily unavailable due to maintenance."
                });
            }

            var stats = new
            {
                events = _eventsCount,
                orders = _ordersCount,
                users = _usersCount
            };

            return Ok(stats);
        }

    }
}
