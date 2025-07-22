using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibraryDATA.ViewModels
{
    public class OrdersView
    {
        public int OdId { get; set; }
        public int Id { get; set; }
        public string UserName { get; set; }
        public string? OrderStatus { get; set; }
        public string? PaymentMethod { get; set; }
        public DateTime? CreatedAt { get; set; }
        public decimal TotalBill => OrderDetails.Sum(o => o.TotalPrice ?? (o.Price * o.Quantity));
        public List<OrderItemView> OrderDetails { get; set; } = new List<OrderItemView>();
    }
    public class OrderItemView
    {
        public string FoodName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal? TotalPrice { get; set; }
    }
}
