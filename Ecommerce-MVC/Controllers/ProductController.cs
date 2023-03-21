using Ecommerce_MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Text;

namespace Ecommerce_MVC.Controllers
{
    public class ProductController : Controller
    {
        private readonly ISession session;
        public ProductController(IHttpContextAccessor httpContextAccessor)
        {
            session = httpContextAccessor.HttpContext.Session;
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                var email = session.GetString("Ausername");
                if (email != null && email != "")
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
                    return View(productFromAPI);
                }
                else
                {
                    return RedirectToAction("Login", "Admin");
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("Error", "Customer", new { msg = e.InnerException.Message });
            }
        }

        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                var email = session.GetString("Ausername");
                if (email != null && email != "")
                {
                    List<Product> prodDetsFromAPI = new();
                    using (var httpClient = new HttpClient())
                    {
                        using (var response = await httpClient.GetAsync("https://localhost:7100/api/Products"))
                        {
                            string apiResponse = await response.Content.ReadAsStringAsync();
                            prodDetsFromAPI = JsonConvert.DeserializeObject<List<Product>>(apiResponse);
                        }
                    }
                    var res = prodDetsFromAPI.Where(x => x.Pid == id)
                        .FirstOrDefault();

                    if (res != null)
                    {
                        return View(res);
                    }
                    else
                    {
                        return RedirectToAction("Login", "Login");
                    }
                }
                else
                {
                    return RedirectToAction("Login", "Admin");
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("Error", "Customer", new { msg = e.InnerException.Message });
            }
        }
        // GET: Products/Create
        public async Task<IActionResult> Create()
        {
            try
            {
                var email = session.GetString("Ausername");
                if (email != null && email != "")
                {
                    List<int> Temp = new();
                    List<Brand> brandsFromAPI = new();
                    using (var httpClient = new HttpClient())
                    {
                        using (var response = await httpClient.GetAsync("https://localhost:7100/api/Brands"))
                        {
                            string apiResponse = await response.Content.ReadAsStringAsync();
                            brandsFromAPI = JsonConvert.DeserializeObject<List<Brand>>(apiResponse);
                        }
                    }
                    foreach(var item in brandsFromAPI)
                    {
                        Temp.Add(item.Bid);
                    }
                    ViewBag.BrandId = new SelectList(Temp);
                    return View();
                }
                else
                {
                    return RedirectToAction("Login", "Admin");
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("Error", "Customer", new { msg = e.InnerException.Message });
            }
        }

        // POST: Products/Create
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            try
            {
                var email = session.GetString("Ausername");
                if (email != null && email != "")
                {
                    Product productFromAPI = new();
                    using (var httpClient = new HttpClient())
                    {
                        StringContent valuesToAdd = new StringContent(JsonConvert.SerializeObject(product),
                      Encoding.UTF8, "application/json");

                        using (var response = await httpClient.PostAsync("https://localhost:7100/api/Products/", valuesToAdd))
                        {
                            string apiResponse = await response.Content.ReadAsStringAsync();
                            if (apiResponse.Contains("already"))
                            {
                                throw new Exception("Product ID already exists");
                            }
                            productFromAPI = JsonConvert.DeserializeObject<Product>(apiResponse);
                        }
                    }
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Login", "Admin");
                }
            }
            catch (Exception e)
            {
                ViewBag.emsg = e.Message;
                return View();
            }
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                var email = session.GetString("Ausername");
                if (email != null && email != "")
                {
                    Product productFromAPI = new Product();
                    using (var httpClient = new HttpClient())
                    {
                        using (var response = await httpClient.GetAsync("https://localhost:7100/api/Products/" + id))
                        {
                            string apiResponse = await response.Content.ReadAsStringAsync();
                            productFromAPI = JsonConvert.DeserializeObject<Product>(apiResponse);
                        }
                    }
                    return View(productFromAPI);
                }
                else
                {
                    return RedirectToAction("Login", "Admin");
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("Error", "Customer", new { msg = e.InnerException.Message });
            }
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,Product product)
        {
            try
            {
                var email = session.GetString("Ausername");
                if (email != null && email != "")
                {
                    Product productFromAPI = new();
                    using (var httpClient = new HttpClient())
                    {
                        StringContent valuesToAdd = new StringContent(JsonConvert.SerializeObject(product),
                      Encoding.UTF8, "application/json");

                        using (var response = await httpClient.PutAsync("https://localhost:7100/api/Products/" + id, valuesToAdd))
                        {
                            string apiResponse = await response.Content.ReadAsStringAsync();
                            productFromAPI = JsonConvert.DeserializeObject<Product>(apiResponse);
                        }
                    }
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Login", "Admin");
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("Error", "Customer", new { msg = e.InnerException.Message });
            }
        }
    }       
}
