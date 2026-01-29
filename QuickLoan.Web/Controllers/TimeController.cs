using Microsoft.AspNetCore.Mvc;

namespace QuickLoan.Web.Controllers
{
    public class TimeController : Controller
    {
        [HttpPost]
        public IActionResult SetTimeZone([FromBody] TimeZoneRequest request)
        {
            HttpContext.Session.SetString("UserTimeZone", request.TimeZone);
            return Ok();
        }
    }

    public record TimeZoneRequest(string TimeZone);
}
