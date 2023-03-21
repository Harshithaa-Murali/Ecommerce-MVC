using Ecommerce_MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace Ecommerce_MVC.Controllers
{
    public class OrderController : Controller
    {
        private readonly ISession session;
        public OrderController(IHttpContextAccessor httpContextAccessor)
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
                    List<Order> productFromAPI = new();
                    using (var httpClient = new HttpClient())
                    {
                        using (var response = await httpClient.GetAsync("https://localhost:7100/api/Orders"))
                        {
                            string apiResponse = await response.Content.ReadAsStringAsync();
                            productFromAPI = JsonConvert.DeserializeObject<List<Order>>(apiResponse);
                        }
                    }
                    return View(productFromAPI);
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
        public async Task<IActionResult> Details(int? id)
        {
            var email = session.GetString("Ausername");
            if (email != null && email!= "")
            {
                try
                {
                    /*var email = HttpContext.Session.GetString("username");*/

                    List<Order> orderFromAPI = new();
                    using (var httpClient = new HttpClient())
                    {
                        using (var response = await httpClient.GetAsync("https://localhost:7100/api/Orders"))
                        {
                            string apiResponse = await response.Content.ReadAsStringAsync();
                            orderFromAPI = JsonConvert.DeserializeObject<List<Order>>(apiResponse);
                        }
                    }
                    var res = orderFromAPI.Where(x => x.OrderId == id)
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
      
        // POST: Products/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(bool Ps, double oid, int mop)
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
                    int x = un.Cid;

                    Order order = new Order();
                    List<int> temp = new();
                    Random r = new Random();
                    order.OrderId = r.Next(10000, 50000);
                    order.Invoice = Convert.ToString(r.Next(230000000, 728294040));
                    order.PendingStatus = Ps;
                    order.BillAmmount = oid;
                    order.CustId = x;
                    order.DateOfOrder = DateTime.Now;

                    for (int i = 0; i < CategoryVM.cart.Count; i++)
                    {
                        temp.Add(CategoryVM.Qty[CategoryVM.cart[i].Pid]);
                        while (CategoryVM.Qty[CategoryVM.cart[i].Pid] > 0)
                        {
                            int p = CategoryVM.cart[i].Pid;
                            CategoryVM.cart[i].InStock--;

                            Product z = new Product();
                            using (var httpClient = new HttpClient())
                            {
                                using (var response = await httpClient.GetAsync("https://localhost:7100/api/Products/" + p))
                                {
                                    string apiResponse = await response.Content.ReadAsStringAsync();
                                    z = JsonConvert.DeserializeObject<Product>(apiResponse);
                                }
                            }
                            z.InStock--;
                            using (var httpClient = new HttpClient())
                            {
                                StringContent valuesToAdd = new StringContent(JsonConvert.SerializeObject(z),
                              Encoding.UTF8, "application/json");

                                using (var response = await httpClient.PutAsync("https://localhost:7100/api/Products/" + p, valuesToAdd))
                                {
                                    string apiResponse = await response.Content.ReadAsStringAsync();
                                    z = JsonConvert.DeserializeObject<Product>(apiResponse);
                                }
                            }

                            CategoryVM.Qty[CategoryVM.cart[i].Pid]--;
                        }
                    }
                    Order orderFromAPI = new();
                    using (var httpClient = new HttpClient())
                    {
                        StringContent valuesToAdd = new StringContent(JsonConvert.SerializeObject(order),
                      Encoding.UTF8, "application/json");

                        using (var response = await httpClient.PostAsync("https://localhost:7100/api/Orders/", valuesToAdd))
                        {
                            string apiResponse = await response.Content.ReadAsStringAsync();
                            orderFromAPI = JsonConvert.DeserializeObject<Order>(apiResponse);
                        }
                    }

                    int k = 0;
                    foreach (var item in CategoryVM.cart)
                    {
                        OrderDetail orderdetsFromAPI = new();
                        orderdetsFromAPI.OrderId = order.OrderId;
                        orderdetsFromAPI.Pid = item.Pid;
                        orderdetsFromAPI.Qty = temp[k];
                        using (var httpClient = new HttpClient())
                        {
                            StringContent valuesToAdd = new StringContent(JsonConvert.SerializeObject(orderdetsFromAPI),
                          Encoding.UTF8, "application/json");

                            using (var response = await httpClient.PostAsync("https://localhost:7100/api/OrderDetails/", valuesToAdd))
                            {
                                string apiResponse = await response.Content.ReadAsStringAsync();
                                orderdetsFromAPI = JsonConvert.DeserializeObject<OrderDetail>(apiResponse);
                            }
                        }
                        k++;
                    }

                    CategoryVM.cart.Clear();
                    CategoryVM.Qty.Clear();
                    return RedirectToAction("OrderConfirmed");
                }
                catch (Exception ex)
                {
                    return RedirectToAction("Error", "Customer", new { msg = ex.Message });
                }
            }
            else
            {
                return RedirectToAction("login", "login");
            }
        }
        public IActionResult OrderConfirmed()
        {
            return View();
        }
        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var email = session.GetString("Ausername");
            if (email != null && email!= "")
            {
                try
                {
                    Order orderFromAPI = new Order();
                    using (var httpClient = new HttpClient())
                    {
                        using (var response = await httpClient.GetAsync("https://localhost:7100/api/Orders/" + id))
                        {
                            string apiResponse = await response.Content.ReadAsStringAsync();
                            orderFromAPI = JsonConvert.DeserializeObject<Order>(apiResponse);
                        }
                    }
                    return View(orderFromAPI);
                }
                catch (Exception ex)
                {
                    return RedirectToAction("Error", "Customer", new { msg = ex.InnerException.Message });
                }
            }
            else 
            {
                return RedirectToAction("login", "admin");
            }
        }

        // POST: Products/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Order order)
        {
            var email = session.GetString("Ausername");
            if(email!=null && email!= "")
            {
                try
                {
                    Order orderFromAPI = new();
                    using (var httpClient = new HttpClient())
                    {
                        StringContent valuesToAdd = new StringContent(JsonConvert.SerializeObject(order),
                      Encoding.UTF8, "application/json");

                        using (var response = await httpClient.PutAsync("https://localhost:7100/api/Orders/" + id, valuesToAdd))
                        {
                            string apiResponse = await response.Content.ReadAsStringAsync();
                            orderFromAPI = JsonConvert.DeserializeObject<Order>(apiResponse);
                        }
                    }
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    return RedirectToAction("Error", "Customer", new { msg = ex.InnerException.Message });
                }
            }
            else 
            {
                return RedirectToAction("login", "admin");
            }
        }
    }
}
