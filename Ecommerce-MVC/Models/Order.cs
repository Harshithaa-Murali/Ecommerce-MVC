namespace Ecommerce_MVC.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public string? Invoice { get; set; }
        public bool? PendingStatus { get; set; }
        public double BillAmmount { get; set; }
        public int CustId { get; set; }
        public DateTime DateOfOrder { get; set; }
        public virtual Login Cust { get; set; } = null!;
    }
}
