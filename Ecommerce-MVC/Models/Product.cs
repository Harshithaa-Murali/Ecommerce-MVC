using System.Drawing.Drawing2D;

namespace Ecommerce_MVC.Models
{
    public class Product
    {
        public int Pid { get; set; }
        public string? Category { get; set; }
        public string? SubCategory { get; set; }
        public string? ScType { get; set; }
        public string? Size { get; set; }
        public int? BrandId { get; set; }
        public int? InStock { get; set; }
        public double? Price { get; set; }
        public string? Imglink { get; set; }

        public virtual Brand? Brand { get; set; }
    }
}
