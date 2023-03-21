namespace Ecommerce_MVC.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }
        public int? OrderId { get; set; }
        public int? Pid { get; set; }

        public int? Qty { get; set; }
        public virtual Order? Order { get; set; }
        public virtual Product? PidNavigation { get; set; }
    }
}
