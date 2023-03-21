using Ecommerce_MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections;

namespace Ecommerce_MVC.Controllers
{

    public class LoginController : Controller
    {
        private readonly ISession session;
        public LoginController(IHttpContextAccessor httpContextAccessor)
        {
            session = httpContextAccessor.HttpContext.Session;
        }
        public IActionResult Login()
        {
            session.SetString("username", "");
            session.SetString("Ausername", "");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(Login login)
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

                List<Login> loginFromAPI = new();
                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.GetAsync("https://localhost:7100/api/Logins"))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        loginFromAPI = JsonConvert.DeserializeObject<List<Login>>(apiResponse);
                    }
                }
                var res = loginFromAPI.Where(x => x.Username == login.Username && x.Pwd == login.Pwd)
                    .FirstOrDefault();


                if (res != null)
                {
                    session.SetString("username", login.Username);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.ErrorMessage = "Incorrect username/password";
                    return View();
                }
            }
            catch(Exception e)
            {
                return RedirectToAction("Error", "Customer", new { msg = e.InnerException.Message });
            }
        }
        public IActionResult Logout()
        {
            session.Clear();
            CategoryVM.cart.Clear();
            CategoryVM.st.Clear();
            CategoryVM.Qty.Clear();
            return RedirectToAction("Login");
        }
    }
}
