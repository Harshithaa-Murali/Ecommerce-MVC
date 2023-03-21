using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace Ecommerce_MVC.Models
{
    public class Login
    {
        public string? Username { get; set; }

        [Required(ErrorMessage ="This field is required")]
        [RegularExpression("[A-Za-z0-9!#$%^&*]{8,20}",ErrorMessage ="Password not in the given range")]
        public string Pwd { get; set; } = null!;
        public int Cid { get; set; }
        public string? Cname { get; set; }
        public string? Email { get; set; }
        public string? PhoneNo { get; set; }
    }
}
