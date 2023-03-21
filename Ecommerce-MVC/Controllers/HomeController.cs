using Ecommerce_MVC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections;
using System.Diagnostics;

namespace Ecommerce_MVC.Controllers
{
    public class HomeController : Controller
    {
        
        private readonly ISession session;
        List<Product> pro = new List<Product>();
        public HomeController(IHttpContextAccessor httpContextAccessor)
        {
            session = httpContextAccessor.HttpContext.Session;
        }

        public IActionResult Index()
        {
            var email = session.GetString("username");
            if (email != null && email!= "")
            {
                try
                {
                    return View();
                }
                catch (Exception ex)
                {
                    return RedirectToAction("Error", "Customer", new { msg = ex.Message });
                }
            }
            else
            {
                return RedirectToAction("Login", "Login");
            } 
        }
        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> DisplayList(int disc)
        {
            var email = session.GetString("username");
            if (email != null && email!= "")
            {
                try
                {
                    ViewBag.disc = disc;
                    ViewBag.bnames = CategoryVM.brands;

                    return View(CategoryVM.st);
                }
                catch (Exception ex)
                {
                    return RedirectToAction("Error", "Customer", new { msg = ex.Message });
                }
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }
        public IActionResult MyCart(int disc)
        {
            try
            {
                var email = HttpContext.Session.GetString("username");
                if (email != null && email!="")
                {
                    ViewBag.disc = disc;
                    ViewBag.Cart = "Cart ";
                    ViewBag.qty = CategoryVM.Qty;
                    return View(CategoryVM.cart);
                }
                else
                {
                    return RedirectToAction("login", "login");
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Customer", new { msg = ex.Message });
            }
        }
        public async Task<IActionResult> AddToCart(int id, int disc)
        {
            try
            {
                var email = HttpContext.Session.GetString("username");
                if (email != null && email!="")
                {
                    ViewBag.disc = disc;

                    List<Product> prodDetsFromAPI = new();
                    using (var httpClient = new HttpClient())
                    {
                        using (var response = await httpClient.GetAsync("https://localhost:7100/api/Products"))
                        {
                            string apiResponse = await response.Content.ReadAsStringAsync();
                            prodDetsFromAPI = JsonConvert.DeserializeObject<List<Product>>(apiResponse);
                        }
                    }
                    var item = prodDetsFromAPI.Where(x => x.Pid == id)
                        .FirstOrDefault();

                    if (!CategoryVM.Qty.ContainsKey(id))
                    {
                        CategoryVM.cart.Add(item);
                        CategoryVM.Qty.Add(id, 0);
                    }
                    else if (CategoryVM.Qty[id] == 0)
                    {
                        CategoryVM.cart.Add(item);
                    }
                    CategoryVM.Qty[id] = CategoryVM.Qty[id] + 1;
                    foreach (var xy in CategoryVM.st)
                    {
                        foreach (var z in xy.Prods) 
                        {
                            if (z.Pid == id)
                            {
                                z.InStock--;
                            }
                        }
                    }
                    pro.Add(item);
                    ViewBag.message = "Product added to Cart !";
                    return View(pro);
                }
                else
                {
                    return RedirectToAction("login", "login");
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Customer", new { msg = ex.Message });
            }
        }
        public IActionResult RemoveFromCart(int id)
        {
            try
            {
                var email = HttpContext.Session.GetString("username");
                if (email != null && email!="")
                {
                    ViewBag.Rid = id;
                    var result = (from i in CategoryVM.cart
                                  where i.Pid == ViewBag.Rid
                                  select i).ToList();
                    int key1 = (from i in CategoryVM.Qty
                                where i.Key == ViewBag.Rid
                                select i.Key).FirstOrDefault();
                    foreach (var xy in CategoryVM.st)
                    {
                        foreach (var z in xy.Prods)
                        {
                            if (z.Pid == id)
                            {
                                z.InStock++;
                            }
                        }
                    }
                    if (CategoryVM.Qty[key1] == 1)
                    {
                        foreach (var item in result)
                        {
                            CategoryVM.cart.Remove(item);
                        }
                        CategoryVM.Qty[key1] = CategoryVM.Qty[key1] - 1;
                    }
                    else
                    {
                        CategoryVM.Qty[key1] = CategoryVM.Qty[key1] - 1;
                    }
                    ViewBag.qty = CategoryVM.Qty;
                    return RedirectToAction("MyCart");
                }
                else
                {
                    return RedirectToAction("login", "login");
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Customer", new { msg = ex.Message });
            }
        }
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}