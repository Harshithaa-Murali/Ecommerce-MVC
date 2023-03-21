namespace Ecommerce_MVC.Models
{
    public class Brand
    {
        public Brand()
        {
            Products = new HashSet<Product>();
        }

        public int Bid { get; set; }
        public string? BrandName { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
