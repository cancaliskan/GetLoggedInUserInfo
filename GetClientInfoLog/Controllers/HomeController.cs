using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using GetClientInfoLog.Models;

namespace GetClientInfoLog.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public IActionResult Index(ClientLogInfo model)
        {
            return View(model);
        }
    }
}