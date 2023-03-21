using Ecommerce_MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace Ecommerce_MVC.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ISession session;
        public CustomerController(IHttpContextAccessor httpContextAccessor)
        {
            session = httpContextAccessor.HttpContext.Session;
        }
        public async Task<IActionResult> Index()
        {
            var email = session.GetString("Ausername");
            if (email != null && email!= "")
            {
                try
                {
                    List<Login> customerFromAPI = new();
                    using (var httpClient = new HttpClient())
                    {
                        using (var response = await httpClient.GetAsync("https://localhost:7100/api/Logins"))
                        {
                            string apiResponse = await response.Content.ReadAsStringAsync();
                            customerFromAPI = JsonConvert.DeserializeObject<List<Login>>(apiResponse);
                        }
                    }
                    return View(customerFromAPI);
                }
                catch (Exception ex)
                {
                    return RedirectToAction("Error", "Customer", new { msg = ex.InnerException.Message });
                }
            }
            else
            {
                return RedirectToAction("Login", "Admin");
            } 
        }
        public IActionResult Create()
        {
            return View();
        }
        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Login login)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Login customerFromAPI = new();
                    using (var httpClient = new HttpClient())
                    {
                        StringContent valuesToAdd = new StringContent(JsonConvert.SerializeObject(login),
                      Encoding.UTF8, "application/json");

                        using (var response = await httpClient.PostAsync("https://localhost:7100/api/Logins/", valuesToAdd))
                        {
                            string apiResponse = await response.Content.ReadAsStringAsync();
                            if (apiResponse.Contains("already"))
                            {
                                throw new Exception("Username/Phone number already exists");
                            }
                            customerFromAPI = JsonConvert.DeserializeObject<Login>(apiResponse);
                        }
                    }
                    return RedirectToAction("RegisterConfirmed");
                }
                else
                {
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.exmsg = ex.Message;
                return View();
            }
        }
        public IActionResult RegisterConfirmed()
        {
            return View();
        }
        public async Task<IActionResult> Edit(int? id)
        {
            var email = session.GetString("username");
            if (email != null && email!= "")
            {
                try
                {
                    string uname = HttpContext.Session.GetString("username");
                    Login un = new Login();
                    using (var httpClient = new HttpClient())
                    {
                        using (var response = await httpClient.GetAsync("https://localhost:7100/api/Logins/un/" + uname))
                        {
                            string apiResponse = await response.Content.ReadAsStringAsync();
                            un = JsonConvert.DeserializeObject<Login>(apiResponse);
                        }
                    }
                    Login customerFromAPI = new Login();
                    using (var httpClient = new HttpClient())
                    {
                        using (var response = await httpClient.GetAsync("https://localhost:7100/api/Logins/" + un.Cid))
                        {
                            string apiResponse = await response.Content.ReadAsStringAsync();
                            customerFromAPI = JsonConvert.DeserializeObject<Login>(apiResponse);
                        }
                    }
                    ViewBag.cid = un.Cid;
                    return View(customerFromAPI);
                }
                catch (Exception ex)
                {
                    return RedirectToAction("Error", "Customer", new { msg = ex.InnerException.Message });
                }
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Login login)
        {
            var email = session.GetString("username");
            if (email != null && email!= "")
            {
                try
                {
                    Login customerFromAPI = new();
                    using (var httpClient = new HttpClient())
                    {
                        StringContent valuesToAdd = new StringContent(JsonConvert.SerializeObject(login),
                      Encoding.UTF8, "application/json");

                        using (var response = await httpClient.PutAsync("https://localhost:7100/api/Logins/" + id, valuesToAdd))
                        {
                            string apiResponse = await response.Content.ReadAsStringAsync();
                            if (apiResponse.Contains("Already"))
                            {
                                throw new Exception("Email/Phone number already exists");
                            }
                            customerFromAPI = JsonConvert.DeserializeObject<Login>(apiResponse);
                            if (response.IsSuccessStatusCode)
                            {
                                ViewBag.smsg = "Account details updated !";
                            }
                        }
                    }
                    return View();
                }
                catch (Exception ex)
                {
                    ViewBag.emsg = ex.Message;
                    return View();
                }
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        public async Task<IActionResult> OrderDetails()
        {
            var email = session.GetString("username");
            if (email != null && email!= "")
            {
                try
                {
                    string uname = HttpContext.Session.GetString("username");
                    Login un = new Login();
                    using (var httpClient = new HttpClient())
                    {
                        using (var response = await httpClient.GetAsync("https://localhost:7100/api/Logins/un/" + uname))
                        {
                            string apiResponse = await response.Content.ReadAsStringAsync();
                            un = JsonConvert.DeserializeObject<Login>(apiResponse);
                        }
                    }
                    List<Order> ordersFromAPI = new List<Order>();
                    using (var httpClient = new HttpClient())
                    {
                        using (var response = await httpClient.GetAsync("https://localhost:7100/api/Orders/cid/" + un.Cid))
                        {
                            string apiResponse = await response.Content.ReadAsStringAsync();
                            ordersFromAPI = JsonConvert.DeserializeObject<List<Order>>(apiResponse);
                        }
                    }
                    List<OrderDetail> orderdetsFromAPI = new();
                    using (var httpClient = new HttpClient())
                    {
                        using (var response = await httpClient.GetAsync("https://localhost:7100/api/OrderDetails/"))
                        {
                            string apiResponse = await response.Content.ReadAsStringAsync();
                            orderdetsFromAPI = JsonConvert.DeserializeObject<List<OrderDetail>>(apiResponse);
                        }
                    }
                    ViewBag.prod = orderdetsFromAPI;
                    return View(ordersFromAPI);
                }
                catch (Exception ex)
                {
                    return RedirectToAction("Error", "Customer", new { msg = ex.InnerException.Message });
                }
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }
        public IActionResult error(string msg)
        {
            ViewBag.emsg = msg;
            return View();
        }
    }
        
}
