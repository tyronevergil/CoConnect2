using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoConnect.Controllers
{
    [Authorize]
    public class ContactsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}