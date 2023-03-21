using Ecommerce_MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections;

namespace Ecommerce_MVC.Controllers
{
    public class AdminController : Controller
    {
        private readonly ISession session;
        public AdminController(IHttpContextAccessor httpContextAccessor)
        {
            session = httpContextAccessor.HttpContext.Session;
        }
        public IActionResult Login()
        {
            session.SetString("Ausername", "");
            session.SetString("username", "");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(Admin admin)
        {
            try
            {
                List<Product> productFromAPI = new();
                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.GetAsync("https://localhost:7100/api/Products"))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        productFromAPI = JsonConvert.DeserializeObject<List<Product>>(apiResponse);
                    }
                }
                List<Brand> brandsFromAPI = new();
                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.GetAsync("https://localhost:7100/api/Brands"))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        brandsFromAPI = JsonConvert.DeserializeObject<List<Brand>>(apiResponse);
                    }
                }
                var items = productFromAPI
                    .GroupBy(u => u.Category, (key, items) => new CategoryVM
                    {
                        Category = key,
                        Prods = items.ToList()
                    })

                    .ToList();

                CategoryVM.st = items;
                ArrayList temp = new ArrayList();
                foreach (var item in CategoryVM.st)
                {
                    List<string> l3 = new List<string>();
                    foreach (var i in item.Prods)
                    {
                        var r = brandsFromAPI.Where(x => x.Bid == i.BrandId).Select(x => x.BrandName).FirstOrDefault();
                        l3.Add(r);
                    }
                    temp.Add(l3);
                }
                CategoryVM.brands = temp;

                List<Admin> adminFromAPI = new();
                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.GetAsync("https://localhost:7100/api/Admins"))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        adminFromAPI = JsonConvert.DeserializeObject<List<Admin>>(apiResponse);
                    }
                }
                var res = adminFromAPI.Where(x => x.Username == admin.Username && x.Pwd == admin.Pwd)
                    .FirstOrDefault();

                if (res != null)
                {
                    session.SetString("Ausername", res.Username);
                    return RedirectToAction("Home");
                }
                else
                {
                    ViewBag.ErrorMessage = "Incorrect username/password";
                    return View();
                }
            }
            catch(Exception e)
            {
                return RedirectToAction("Error", "Customer", new {msg = e.InnerException.Message});
            }
        }
        public IActionResult Home()
        {
            try
            {
                var uname = session.GetString("Ausername");
                if (uname != null)
                {
                    return RedirectToAction("Index", "Product");
                }
                else
                {
                    return RedirectToAction("login", "admin");
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("Error", "Customer", new { msg = e.InnerException.Message });
            }
        }
        public IActionResult Logout()
        {
            session.Clear();
            return RedirectToAction("Login","Admin");
        }
    }
}
