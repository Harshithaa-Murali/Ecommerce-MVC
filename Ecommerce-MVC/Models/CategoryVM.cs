using System.Collections;

namespace Ecommerce_MVC.Models
{
    public class CategoryVM
    {
        public string Category { get; set; }
        public static List<CategoryVM> st = new List<CategoryVM>();
        public static ArrayList brands = new ArrayList();
        public List<Product> Prods { get; set; }

        public static List<Product> cart = new List<Product>();
        public static Dictionary<int, int> Qty = new Dictionary<int, int>();
        public List<Product> mystock = new List<Product>();

        
    }
}
