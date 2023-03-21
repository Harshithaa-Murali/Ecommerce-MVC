using Microsoft.AspNetCore.Mvc;

namespace Ecommerce_MVC.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly ISession session;
        public PaymentsController(IHttpContextAccessor httpContextAccessor)
        {
            session = httpContextAccessor.HttpContext.Session;
        }
        public IActionResult ModeOfPayment(double oid, string pids)
        {
            try
            {
                var email = session.GetString("username");
                if (email != null && email!="")
                {
                    ViewBag.oid = oid;
                    ViewBag.pids = pids;
                    return View();
                }
                else
                {
                    return RedirectToAction("Login", "Login");
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Customer", new { msg = ex.InnerException.Message });
            }
        }
    }
}
