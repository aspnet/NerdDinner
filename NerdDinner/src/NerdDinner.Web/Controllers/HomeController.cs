using Microsoft.AspNet.Mvc;

namespace NerdDinner.Web.Controllers
{
    [Route("/")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}